using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.LTI;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class QuizController : Controller
	{
		private static ILog log = LogManager.GetLogger(typeof(QuizController));

		private const int MAX_DROPS_COUNT = 1;
		public const int MAX_FILLINBLOCK_SIZE = 1024;

		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly QuizzesRepo quizzesRepo = new QuizzesRepo();
		private readonly GroupsRepo groupsRepo = new GroupsRepo();
		private readonly SlideCheckingsRepo slideCheckingsRepo = new SlideCheckingsRepo();

		public QuizController()
			: this(WebCourseManager.Instance)
		{
		}

		public QuizController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		internal class QuizAnswer
		{
			public string QuizType;
			public string QuizId;
			public string ItemId;
			public string Text;

			public QuizAnswer(string type, string quizId, string itemId, string text)
			{
				QuizType = type;
				QuizId = quizId;
				ItemId = itemId;
				Text = text;
			}
		}


		internal class QuizInfoForDb
		{
			public string QuizId;
			public string ItemId;
			public string Text;
			public bool IsRightAnswer;
			public Type QuizType;
			public int QuizBlockScore;
			public int QuizBlockMaxScore;
		}

		public bool CanUserFillQuiz(QuizState state)
		{
			return state == QuizState.NotPassed || state == QuizState.WaitForCheck;
		}

		[AllowAnonymous]
		public ActionResult Quiz(QuizSlide slide, string courseId, string userId, bool isGuest, bool isLti = false, ManualQuizChecking manualQuizCheckQueueItem = null, int? groupId = null)
		{
			if (isGuest)
				return PartialView(GuestQuiz(slide, courseId));
			var slideId = slide.Id;
			var maxDropCount = GetMaxDropCount(slide);
			var state = GetQuizState(courseId, userId, slideId, maxDropCount);
			var quizState = state.Item1;
			var tryNumber = state.Item2;
			var resultsForQuizes = GetResultForQuizes(courseId, userId, slideId, state.Item1);

			log.Info($"Создаю тест для пользователя {userId} в слайде {courseId}:{slide.Id}, isLti = {isLti}");

			var quizVersion = quizzesRepo.GetLastQuizVersion(courseId, slideId);
			if (quizState != QuizState.NotPassed)
				quizVersion = userQuizzesRepo.FindQuizVersionFromUsersAnswer(courseId, slideId, userId);

			/* If we haven't quiz version in database, create it */
			if (quizVersion == null)
				quizVersion = quizzesRepo.AddQuizVersionIfNeeded(courseId, slide);

			/* Restore quiz slide from version stored in the database */
			var quiz = quizVersion.RestoredQuiz;
			slide = new QuizSlide(slide.Info, quiz);

			var userAnswers = userQuizzesRepo.GetAnswersForShowOnSlide(courseId, slide, userId);
			var canUserFillQuiz = CanUserFillQuiz(quizState);

			var model = new QuizModel
			{
				CourseId = courseId,
				Slide = slide,
				QuizState = quizState,
				TryNumber = tryNumber,
				MaxDropCount = maxDropCount,
				ResultsForQuizes = resultsForQuizes,
				AnswersToQuizes = userAnswers,
				IsLti = isLti,
				ManualQuizCheckQueueItem = manualQuizCheckQueueItem,
				CanUserFillQuiz = canUserFillQuiz,
				GroupId = groupId,
			};

			if (model.QuizState != QuizState.NotPassed && model.Score == quiz.MaxScore)
				model.QuizState = QuizState.Total;

			return PartialView(model);
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Tester)]
		public async Task<ActionResult> ClearAnswers(string courseId, Guid slideId, bool isLti)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			var userId = User.Identity.GetUserId();
			await userQuizzesRepo.RemoveAnswers(userId, slideId);
			await visitsRepo.RemoveAttempts(slideId, userId);
			var model = new { courseId, slideId = slide.Id };
			if (isLti)
			{
				LtiUtils.SubmitScore(slide, userId);
				return RedirectToAction("LtiSlide", "Course", model);
			}
			return RedirectToAction("SlideById", "Course", model);
		}

		[HttpPost]
		public async Task<ActionResult> SubmitQuiz(string courseId, Guid slideId, string answer, bool isLti)
		{
			var course = courseManager.GetCourse(courseId);
			var slide = (QuizSlide) course.FindSlideById(slideId);
			if (slide == null)
				return new HttpNotFoundResult();

			var userId = User.Identity.GetUserId();
			var maxDropCount = GetMaxDropCount(slide);
			var quizState = GetQuizState(courseId, userId, slideId, maxDropCount).Item1;
			if (! CanUserFillQuiz(quizState))
				return new HttpStatusCodeResult(HttpStatusCode.OK, "Already answered");

			var time = DateTime.Now;
			var answers = JsonConvert.DeserializeObject<List<QuizAnswer>>(answer).GroupBy(x => x.QuizId);
			var quizBlockWithTaskCount = slide.Blocks.Count(x => x is AbstractQuestionBlock);
			var allQuizInfos = new List<QuizInfoForDb>();
			foreach (var ans in answers)
			{
				var quizInfos = CreateQuizInfo(slide, ans);
				allQuizInfos.AddRange(quizInfos);
			}
			var blocksInAnswerCount = allQuizInfos.Select(x => x.QuizId).Distinct().Count();
			if (blocksInAnswerCount != quizBlockWithTaskCount)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Has empty blocks");

			using (var transaction = db.Database.BeginTransaction())
			{
				await userQuizzesRepo.RemoveUserQuizzes(courseId, slideId, userId);

				foreach (var quizInfoForDb in allQuizInfos)
					await userQuizzesRepo.AddUserQuiz(courseId, quizInfoForDb.IsRightAnswer, quizInfoForDb.ItemId, quizInfoForDb.QuizId,
						slideId, quizInfoForDb.Text, userId, time, quizInfoForDb.QuizBlockScore, quizInfoForDb.QuizBlockMaxScore);

				transaction.Commit();
			}

			if (slide.Quiz.ManualCheck)
			{
				/* If this quiz is already queued for checking for this user, don't add it to queue again */
				if (quizState != QuizState.WaitForCheck)
					await slideCheckingsRepo.AddQuizAttemptForManualChecking(courseId, slideId, userId);
			}
			else
			{
				var score = allQuizInfos
					.DistinctBy(forDb => forDb.QuizId)
					.Sum(forDb => forDb.QuizBlockScore);
				await slideCheckingsRepo.AddQuizAttemptWithAutomaticChecking(courseId, slideId, userId, score);
				await visitsRepo.UpdateScoreForVisit(courseId, slideId, userId);
				if (isLti)
					LtiUtils.SubmitScore(slide, userId);
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> ScoreQuiz(int id, string nextUrl, string errorUrl = "")
		{
			if (string.IsNullOrEmpty(errorUrl))
				errorUrl = nextUrl;

			using (var transaction = db.Database.BeginTransaction())
			{
				var checking = slideCheckingsRepo.FindManualCheckingById<ManualQuizChecking>(id);

				if (checking.IsChecked)
					return Redirect(errorUrl + "Эта работа уже была проверена");

				if (! checking.IsLockedBy(User.Identity))
					return Redirect(errorUrl + "Эта работа проверяется другим инструктором");

				var answers = userQuizzesRepo.GetAnswersForUser(checking.SlideId, checking.UserId);

				QuizVersion quizVersion;
				/* If there is no user's answers for quiz, get the latest quiz version */
				if (answers.Count == 0)
					quizVersion = quizzesRepo.GetLastQuizVersion(checking.CourseId, checking.SlideId);
				else
				{
					var firstAnswer = answers.FirstOrDefault().Value.FirstOrDefault();
					quizVersion = firstAnswer != null
						? firstAnswer.QuizVersion
						: quizzesRepo.GetFirstQuizVersion(checking.CourseId, checking.SlideId);
				}

				int totalScore = 0;

				foreach (var question in quizVersion.RestoredQuiz.Blocks.OfType<AbstractQuestionBlock>())
				{
					var scoreFieldName = "quiz__score__" + question.Id;
					var scoreStr = Request.Form[scoreFieldName];
					int score;
					/* Invalid form: score isn't integer */
					if (!int.TryParse(scoreStr, out score))
						return Redirect(errorUrl + $"Неверное количество баллов в задании «{question.QuestionIndex}. {question.Text.TruncateWithEllipsis(50)}»");
					/* Invalid form: score isn't from range 0..MAX_SCORE */
					if (score < 0 || score > question.MaxScore)
						return Redirect(errorUrl + $"Неверное количество баллов в задании «{question.QuestionIndex}. {question.Text.TruncateWithEllipsis(50)}»: {score}");

					await userQuizzesRepo.SetScoreForQuizBlock(checking.UserId, checking.SlideId, question.Id, score);
					totalScore += score;
				}

				await slideCheckingsRepo.MarkManualCheckingAsChecked(checking, totalScore);
				await visitsRepo.UpdateScoreForVisit(checking.CourseId, checking.SlideId, checking.UserId);
				transaction.Commit();
			}

			return Redirect(nextUrl);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfo(QuizSlide slide, IGrouping<string, QuizAnswer> answer)
		{
			var block = slide.GetBlockById(answer.Key);
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, answer.First().Text);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer);
			if (block is OrderingBlock)
				return CreateQuizInfoForDb(block as OrderingBlock, answer);
			if (block is MatchingBlock)
				return CreateQuizInfoForDb(block as MatchingBlock, answer);
			return CreateQuizInfoForDb(block as IsTrueBlock, answer);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IGrouping<string, QuizAnswer> data)
		{
			var isTrue = isTrueBlock.IsRight(data.First().ItemId);
			var blockScore = isTrue ? isTrueBlock.MaxScore : 0;
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrue,
					Text = data.First().ItemId,
					QuizType = typeof(IsTrueBlock),
					QuizBlockScore = blockScore,
					QuizBlockMaxScore = isTrueBlock.MaxScore
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(ChoiceBlock choiceBlock, IGrouping<string, QuizAnswer> answers)
		{
			int blockScore;
			if (!choiceBlock.Multiple)
			{
				var answerItemId = answers.First().ItemId;
				var isTrue = choiceBlock.Items.First(x => x.Id == answerItemId).IsCorrect;
				blockScore = isTrue ? choiceBlock.MaxScore : 0;
				return new List<QuizInfoForDb>
				{
					new QuizInfoForDb
					{
						QuizId = choiceBlock.Id,
						ItemId = answerItemId,
						IsRightAnswer = isTrue,
						Text = null,
						QuizType = typeof (ChoiceBlock),
						QuizBlockScore = blockScore,
						QuizBlockMaxScore = choiceBlock.MaxScore
					}
				};
			}
			var ans = answers.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = choiceBlock.Id,
					IsRightAnswer = choiceBlock.Items.Where(y => y.IsCorrect).Any(y => y.Id == x),
					ItemId = x,
					Text = null,
					QuizType = typeof(ChoiceBlock),
					QuizBlockScore = 0,
					QuizBlockMaxScore = choiceBlock.MaxScore
				}).ToList();
			var isRightQuizBlock = ans.All(x => x.IsRightAnswer) &&
						 choiceBlock.Items.Where(x => x.IsCorrect)
							 .Select(x => x.Id)
							 .All(x => ans.Where(y => y.IsRightAnswer).Select(y => y.ItemId).Contains(x));
			blockScore = isRightQuizBlock ? choiceBlock.MaxScore : 0;
			foreach (var info in ans)
				info.QuizBlockScore = blockScore;
			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(OrderingBlock orderingBlock, IGrouping<string, QuizAnswer> answers)
		{
			var ans = answers.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = orderingBlock.Id,
					IsRightAnswer = true,
					ItemId = x,
					Text = null,
					QuizType = typeof(OrderingBlock),
					QuizBlockScore = 0,
					QuizBlockMaxScore = orderingBlock.MaxScore
				}).ToList();

			var isRightQuizBlock = answers.Count() == orderingBlock.Items.Length &&
				answers.Zip(orderingBlock.Items, (answer, item) => answer.ItemId == item.GetHash()).All(x => x);
			var blockScore = isRightQuizBlock ? orderingBlock.MaxScore : 0;
			foreach (var info in ans)
				info.QuizBlockScore = blockScore;

			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(MatchingBlock matchingBlock, IGrouping<string, QuizAnswer> answers)
		{
			var ans = answers.ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = matchingBlock.Id,
					IsRightAnswer = matchingBlock.Matches.FirstOrDefault(m => m.GetHashForFixedItem() == x.ItemId)?.
										GetHashForMovableItem() == x.Text,
					ItemId = x.ItemId,
					Text = x.Text,
					QuizType = typeof(MatchingBlock),
					QuizBlockScore = 0,
					QuizBlockMaxScore = matchingBlock.MaxScore
				}).ToList();

			var isRightQuizBlock = ans.All(x => x.IsRightAnswer);
			var blockScore = isRightQuizBlock ? matchingBlock.MaxScore : 0;
			foreach (var info in ans)
				info.QuizBlockScore = blockScore;

			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(FillInBlock fillInBlock, string data)
		{
			if (data.Length > MAX_FILLINBLOCK_SIZE)
				data = data.Substring(0, MAX_FILLINBLOCK_SIZE);
			var isRightAnswer = true;
			if (fillInBlock.Regexes != null)
				isRightAnswer = fillInBlock.Regexes.Any(regex => regex.Regex.IsMatch(data));
			var blockScore = isRightAnswer ? fillInBlock.MaxScore : 0;
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = isRightAnswer,
					Text = data,
					QuizType = typeof(FillInBlock),
					QuizBlockScore = blockScore,
					QuizBlockMaxScore = fillInBlock.MaxScore
				}
			};
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> MergeQuizVersions(string courseId, Guid slideId, int quizVersionId, int mergeWithQuizVersionId)
		{
			using (var transcation = db.Database.BeginTransaction())
			{
				var quizVersion = db.QuizVersions.FirstOrDefault(x => x.CourseId == courseId && x.SlideId == slideId && x.Id == quizVersionId);
				var mergeWithQuizVersion = db.QuizVersions.FirstOrDefault(x => x.CourseId == courseId && x.SlideId == slideId && x.Id == mergeWithQuizVersionId);
				if (quizVersion == null || mergeWithQuizVersion == null)
					return HttpNotFound();

				foreach (var userQuiz in db.UserQuizzes.Where(x => x.QuizVersionId == mergeWithQuizVersionId))
					userQuiz.QuizVersionId = quizVersionId;

				quizVersion.NormalizedXml = mergeWithQuizVersion.NormalizedXml;

				db.QuizVersions.Remove(mergeWithQuizVersion);

				await db.SaveChangesAsync();

				transcation.Commit();
			}

			return RedirectToAction("SlideById", "Course", new { courseId, slideId });
		}

		[HttpGet]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public ActionResult Analytics(string courseId, Guid slideId, DateTime periodStart)
		{
			var course = courseManager.GetCourse(courseId);
			var quizSlide = (QuizSlide)course.GetSlideById(slideId);
			var quizVersions = quizzesRepo.GetQuizVersions(courseId, quizSlide.Id).ToList();
			var dict = new SortedDictionary<string, List<QuizAnswerInfo>>();
			var passes = db.UserQuizzes
				.Where(q => quizSlide.Id == q.SlideId && !q.isDropped && periodStart <= q.Timestamp)
				.GroupBy(q => q.UserId)
				.Join(db.Users, g => g.Key, user => user.Id, (g, user) => new { UserId = user.Id, user.UserName, UserQuizzes = g.ToList(), QuizVersion = g.FirstOrDefault().QuizVersion })
				.ToList();
			foreach (var pass in passes)
			{
				var slide = quizSlide;
				if (pass.QuizVersion != null)
					slide = new QuizSlide(quizSlide.Info, pass.QuizVersion.RestoredQuiz);
				dict[pass.UserName] = GetUserQuizAnswers(slide, pass.UserQuizzes).ToList();
			}
			var userIds = passes.Select(p => p.UserId).Distinct().ToList();
			var userNameById = passes.ToDictionary(p => p.UserId, p => p.UserName);
			var groups = groupsRepo.GetUsersGroupsNamesAsStrings(courseId, userIds, User).ToDictionary(kv => userNameById[kv.Key], kv => kv.Value);
			var rightAnswersCount = dict.Values
				.SelectMany(list => list
					.Where(info => info.Score == info.MaxScore))
				.GroupBy(arg => arg.Id)
				.ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

			var usersByQuizVersion = passes.GroupBy(p => p.QuizVersion?.Id).ToDictionary(g => g.Key, g => g.Select(u => u.UserName).ToList());
			var usersWaitsForManualCheck = slideCheckingsRepo.GetManualCheckingQueue<ManualQuizChecking>(
				new ManualCheckingQueueFilterOptions { CourseId = courseId, SlidesIds = new List<Guid> { quizSlide.Id } }
			).ToList().Select(i => i.User.UserName).ToImmutableHashSet();

			return PartialView(new QuizAnalyticsModel
			{
				CourseId = courseId,
				SlideId = quizSlide.Id,

				UserAnswers = dict,
				QuizVersions = quizVersions,
				UsersByQuizVersion = usersByQuizVersion,
				RightAnswersCount = rightAnswersCount,
				GroupByUser = groups,
				UsersWaitsForManualCheck = usersWaitsForManualCheck,
			});
		}

		private static IEnumerable<QuizAnswerInfo> GetUserQuizAnswers(QuizSlide slide, IEnumerable<UserQuiz> userQuizzes)
		{
			var answers = userQuizzes.GroupBy(q => q.QuizId).ToDictionary(g => g.Key, g => g.ToList());
			foreach (var block in slide.Blocks.OfType<AbstractQuestionBlock>())
				if (block is FillInBlock)
					yield return GetFillInBlockAnswerInfo(answers, block.Id, block.QuestionIndex);
				else if (block is ChoiceBlock)
					yield return GetChoiceBlockAnswerInfo(answers, (ChoiceBlock)block, block.QuestionIndex);
				else if (block is IsTrueBlock)
					yield return GetIsTrueBlockAnswerInfo(answers, block.Id, block.QuestionIndex);
				else if (block is OrderingBlock)
					yield return GetOrderingBlockAnswerInfo(answers, (OrderingBlock)block, block.QuestionIndex);
				else if (block is MatchingBlock)
					yield return GetMatchingBlockAnswerInfo(answers, (MatchingBlock)block, block.QuestionIndex);
		}

		private static QuizAnswerInfo GetFillInBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuiz>> answers, string quizId, int questionIndex)
		{
			UserQuiz answer = null;
			if (answers.ContainsKey(quizId))
				answer = answers[quizId].FirstOrDefault();
			return new FillInBlockAnswerInfo
			{
				Answer = answer?.Text,
				IsRight = answer != null && answer.IsRightAnswer,
				Score = answer?.QuizBlockScore ?? 0,
				MaxScore = answer?.QuizBlockMaxScore ?? 0,
				Id = questionIndex.ToString()
			};
		}

		private static QuizAnswerInfo GetChoiceBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuiz>> answers, ChoiceBlock block, int questionIndex)
		{
			IEnumerable<UserQuiz> answer = new List<UserQuiz>();
			if (answers.ContainsKey(block.Id))
				answer = answers[block.Id].Where(q => q.ItemId != null);

			var ans = new SortedDictionary<string, bool>();
			foreach (var item in block.Items)
			{
				ans[item.Id] = false;
			}

			var isRight = false;
			var score = 0;
			var maxScore = 0;
			foreach (var quizItem in answer.Where(quizItem => ans.ContainsKey(quizItem.ItemId)))
			{
				isRight = quizItem.IsQuizBlockScoredMaximum;
				ans[quizItem.ItemId] = true;
				score = quizItem.QuizBlockScore;
				maxScore = quizItem.QuizBlockMaxScore;
			}

			return new ChoiceBlockAnswerInfo
			{
				AnswersId = ans,
				Id = questionIndex.ToString(),
				RealyRightAnswer = new HashSet<string>(block.Items.Where(x => x.IsCorrect).Select(x => x.Id)),
				Score = score,
				MaxScore = maxScore,
				IsRight = isRight
			};
		}


		private static QuizAnswerInfo GetIsTrueBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuiz>> answers, string quizId, int questionIndex)
		{
			UserQuiz answer = null;
			if (answers.ContainsKey(quizId))
				answer = answers[quizId].FirstOrDefault();
			return new IsTrueBlockAnswerInfo
			{
				IsAnswered = answer != null,
				Answer = answer != null && answer.Text == "True",
				Id = questionIndex.ToString(),
				IsRight = answer != null && answer.IsRightAnswer,
				Score = answer?.QuizBlockScore ?? 0,
				MaxScore = answer?.QuizBlockMaxScore ?? 0,
			};
		}

		private static QuizAnswerInfo GetOrderingBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuiz>> answers, OrderingBlock block, int questionIndex)
		{
			IEnumerable<UserQuiz> userAnswers = new List<UserQuiz>();
			UserQuiz firstAnswer = null;
			if (answers.ContainsKey(block.Id))
			{
				userAnswers = answers[block.Id].Where(q => q.ItemId != null);
				firstAnswer = userAnswers.FirstOrDefault();
			}

			var answersPositions = userAnswers.Select(
				userAnswer => block.Items.FindIndex(i => i.GetHash() == userAnswer.ItemId)
			).ToList();

			return new OrderingBlockAnswerInfo
			{
				Id = questionIndex.ToString(),
				AnswersPositions = answersPositions,
				Score = firstAnswer?.QuizBlockScore ?? 0,
				MaxScore = firstAnswer?.QuizBlockMaxScore ?? 0,
			};
		}

		private static QuizAnswerInfo GetMatchingBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuiz>> answers, MatchingBlock block, int questionIndex)
		{
			IEnumerable<UserQuiz> userAnswers = new List<UserQuiz>();
			UserQuiz firstAnswer = null;
			if (answers.ContainsKey(block.Id))
			{
				userAnswers = answers[block.Id].Where(q => q.ItemId != null);
				firstAnswer = userAnswers.FirstOrDefault();
			}

			var isRightMatches = new List<bool>();
			foreach (var match in block.Matches)
			{
				var userAnswer = userAnswers.FirstOrDefault(a => a.ItemId == match.GetHashForFixedItem());
				isRightMatches.Add(userAnswer != null && userAnswer.IsRightAnswer);
			}

			return new MatchingBlockAnswerInfo
			{
				Id = questionIndex.ToString(),
				IsRightMatches = isRightMatches,
				Score = firstAnswer?.QuizBlockScore ?? 0,
				MaxScore = firstAnswer?.QuizBlockMaxScore ?? 0,
			};
		}

		[HttpPost]
		public async Task<ActionResult> DropQuiz(string courseId, Guid slideId, bool isLti)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			if (slide is QuizSlide)
			{
				var userId = User.Identity.GetUserId();
				if (userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).Count(b => b) < GetMaxDropCount(slide as QuizSlide) &&
					!userQuizzesRepo.IsQuizScoredMaximum(courseId, userId, slideId))
				{
					await userQuizzesRepo.DropQuiz(userId, slideId);
					await slideCheckingsRepo.RemoveAttempts(courseId, slideId, userId);
					await visitsRepo.UpdateScoreForVisit(courseId, slideId, userId);
					if (isLti)
						LtiUtils.SubmitScore(slide, userId);
				}
			}
			var model = new { courseId, slideId = slide.Id, isLti };
			if (isLti)
				return RedirectToAction("LtiSlide", "Course", model);
			return RedirectToAction("SlideById", "Course", model);
		}

		private QuizModel GuestQuiz(QuizSlide slide, string courseId)
		{
			return new QuizModel
			{
				CourseId = courseId,
				Slide = slide,
				IsGuest = true,
				QuizState = QuizState.NotPassed
			};
		}

		private static int GetMaxDropCount(QuizSlide quizSlide)
		{
			if (quizSlide == null)
				return MAX_DROPS_COUNT;
			var maxDropCount = quizSlide.MaxDropCount;
			return maxDropCount == 0 ? MAX_DROPS_COUNT : maxDropCount;
		}

		private Dictionary<string, int> GetResultForQuizes(string courseId, string userId, Guid slideId, QuizState state)
		{
			return userQuizzesRepo.GetQuizBlocksTruth(courseId, userId, slideId);
		}

		private Tuple<QuizState, int> GetQuizState(string courseId, string userId, Guid slideId, int maxDropCount)
		{
			var states = userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).ToList();

			var queueItem = userQuizzesRepo.FindManualQuizChecking(courseId, slideId, userId);
			if (queueItem != null)
				return Tuple.Create(queueItem.IsLocked ? QuizState.IsChecking : QuizState.WaitForCheck, states.Count);
			
			if (states.Count > maxDropCount)
				return Tuple.Create(QuizState.Total, states.Count);
			if (states.Any(b => !b))
				return Tuple.Create(QuizState.Subtotal, states.Count);
			return Tuple.Create(QuizState.NotPassed, states.Count);
		}
	}
}