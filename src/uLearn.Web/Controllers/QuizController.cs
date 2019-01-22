using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using JetBrains.Annotations;
using log4net;
using Metrics;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.LTI;
using uLearn.Web.Models;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;
using Ulearn.Core.Extensions;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class QuizController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(QuizController));

		private const int defaultMaxTriesCount = 2;
		public const int InfinityTriesCount = int.MaxValue - 1;
		public const int MaxFillInBlockSize = 8 * 1024;

		private readonly ULearnDb db = new ULearnDb();
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		private readonly GraphiteMetricSender metricSender;

		private readonly UserQuizzesRepo userQuizzesRepo;
		private readonly VisitsRepo visitsRepo;
		private readonly GroupsRepo groupsRepo;
		private readonly SlideCheckingsRepo slideCheckingsRepo;
		private readonly NotificationsRepo notificationsRepo;

		public QuizController()
		{
			metricSender = new GraphiteMetricSender("web");

			userQuizzesRepo = new UserQuizzesRepo(db);
			visitsRepo = new VisitsRepo(db);
			groupsRepo = new GroupsRepo(db, courseManager);
			slideCheckingsRepo = new SlideCheckingsRepo(db);
			notificationsRepo = new NotificationsRepo(db);
		}

		[UsedImplicitly]
		private class QuizAnswer
		{
			public readonly string BlockType;
			public readonly string BlockId;
			public readonly string ItemId;
			public readonly string Text;

			public QuizAnswer(string type, string blockId, string itemId, string text)
			{
				BlockType = type;
				BlockId = blockId;
				ItemId = itemId;
				Text = text;
			}
		}

		private class QuizInfoForDb
		{
			public Type BlockType;			
			public string BlockId;
			public string ItemId;
			public string Text;
			public bool IsRightAnswer;
			public int QuizBlockScore;
			public int QuizBlockMaxScore;
		}

		public bool CanUserFillQuiz(QuizState state)
		{
			return state == QuizState.NotPassed || state == QuizState.WaitForCheck;
		}

		[AllowAnonymous]
		public ActionResult Quiz(QuizSlide slide, string courseId, string userId, bool isGuest, bool isLti = false, ManualQuizChecking manualQuizCheckQueueItem = null, int? send = null)
		{
			metricSender.SendCount("quiz.show");
			if (isLti)
				metricSender.SendCount("quiz.show.lti");
			metricSender.SendCount($"quiz.show.{courseId}");
			metricSender.SendCount($"quiz.show.{courseId}.{slide.Id}");

			var course = courseManager.FindCourse(courseId);
			if (course == null)
				return HttpNotFound();
			
			if (isGuest)
			{
				metricSender.SendCount("quiz.show.to.guest");
				return PartialView(GuestQuiz(course, slide));
			}
			var slideId = slide.Id;
			var maxTriesCount = GetMaxTriesCount(courseId, slide);
			var state = GetQuizState(courseId, userId, slideId);
			
			/* If it's manual checking, change quiz state to IsChecking for correct rendering */
			if (manualQuizCheckQueueItem != null)
				state = Tuple.Create(QuizState.IsChecking, state.Item2);
			
			var quizState = state.Item1;
			var tryNumber = state.Item2;
			var scoresForBlocks = GetScoresForBlocks(courseId, userId, slideId, state.Item1);

			log.Info($"Создаю тест для пользователя {userId} в слайде {courseId}:{slide.Id}, isLti = {isLti}");

			if (quizState == QuizState.Subtotal)
			{
				var score = scoresForBlocks?.AsEnumerable().Sum(res => res.Value) ?? 0;
				/* QuizState.Subtotal is partially obsolete. If user fully solved quiz, then show answers. Else show empty quiz for the new try... */
				if (score == slide.MaxScore)
					quizState = QuizState.Total;
				/* ... and show last try's answers only if argument `send` has been passed in query */
				else if (!send.HasValue)
				{
					quizState = QuizState.NotPassed;
					/* ... if we will show answers from last try then drop quiz */
					userQuizzesRepo.DropQuiz(courseId, slideId, userId);
				}
			}			

			var userAnswers = userQuizzesRepo.GetAnswersForShowingOnSlide(courseId, slide, userId, manualQuizCheckQueueItem?.Submission);
			var canUserFillQuiz = CanUserFillQuiz(quizState);

			var questionAnswersFrequency = new DefaultDictionary<string, DefaultDictionary<string, int>>();
			if (User.HasAccessFor(courseId, CourseRole.CourseAdmin))
			{
				questionAnswersFrequency = slide.Blocks.OfType<ChoiceBlock>().ToDictionary(
					block => block.Id,
					block => userQuizzesRepo.GetAnswersFrequencyForChoiceBlock(courseId, slide.Id, block.Id).ToDefaultDictionary()
				).ToDefaultDictionary();
			}

			var model = new QuizModel
			{
				Course = course,
				Slide = slide,
				QuizState = quizState,
				TryNumber = tryNumber,
				MaxTriesCount = maxTriesCount,
				ScoresForBlocks = scoresForBlocks,
				AnswersToQuizes = userAnswers,
				IsLti = isLti,
				Checking = manualQuizCheckQueueItem,
				CanUserFillQuiz = canUserFillQuiz,
				GroupsIds = Request.GetMultipleValuesFromQueryString("group"),
				QuestionAnswersFrequency = questionAnswersFrequency,
			};

			return PartialView(model);
		}
		
		[HttpPost]
		public async Task<ActionResult> SubmitQuiz(string courseId, Guid slideId, string answer, bool isLti)
		{
			metricSender.SendCount("quiz.submit");
			if (isLti)
				metricSender.SendCount("quiz.submit.lti");
			metricSender.SendCount($"quiz.submit.{courseId}");
			metricSender.SendCount($"quiz.submit.{courseId}.{slideId}");

			var course = courseManager.GetCourse(courseId);
			var slide = course.FindSlideById(slideId) as QuizSlide;
			if (slide == null)
				return new HttpNotFoundResult();

			var userId = User.Identity.GetUserId();
			var maxTriesCount = GetMaxTriesCount(courseId, slide);
			var quizState = GetQuizState(courseId, userId, slideId);
			if (!CanUserFillQuiz(quizState.Item1))
				return new HttpStatusCodeResult(HttpStatusCode.OK, "Already answered");

			var tryIndex = quizState.Item2;
			metricSender.SendCount($"quiz.submit.try.{tryIndex}");
			metricSender.SendCount($"quiz.submit.{courseId}.try.{tryIndex}");
			metricSender.SendCount($"quiz.submit.{courseId}.{slideId}.try.{tryIndex}");

			if (slide.ManualChecking && !groupsRepo.IsManualCheckingEnabledForUser(course, userId))
				return new HttpStatusCodeResult(HttpStatusCode.OK, "Manual checking is disabled for you");

			
			var answers = JsonConvert.DeserializeObject<List<QuizAnswer>>(answer).GroupBy(x => x.BlockId);
			var quizBlockWithTaskCount = slide.Blocks.Count(x => x is AbstractQuestionBlock);
			var allQuizInfos = new List<QuizInfoForDb>();
			foreach (var ans in answers)
			{
				var quizInfos = CreateQuizInfo(slide, ans);
				if (quizInfos != null)
					allQuizInfos.AddRange(quizInfos);
			}
			var blocksInAnswerCount = allQuizInfos.Select(x => x.BlockId).Distinct().Count();
			if (blocksInAnswerCount != quizBlockWithTaskCount)
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Has empty blocks");

			UserQuizSubmission submission;
			using (var transaction = db.Database.BeginTransaction())
			{
				submission = await userQuizzesRepo.AddSubmission(courseId, slideId, userId, DateTime.Now).ConfigureAwait(false);

				foreach (var quizInfoForDb in allQuizInfos)
					await userQuizzesRepo.AddUserQuizAnswer(
						submission.Id,
						quizInfoForDb.IsRightAnswer,
						quizInfoForDb.BlockId,
						quizInfoForDb.ItemId,
						quizInfoForDb.Text,
						quizInfoForDb.QuizBlockScore,
						quizInfoForDb.QuizBlockMaxScore
					).ConfigureAwait(false);

				transaction.Commit();
			}

			if (slide.ManualChecking)
			{
				/* If this quiz is already queued for checking for this user, don't add it to the queue again */
				if (quizState.Item1 != QuizState.WaitForCheck)
				{
					await slideCheckingsRepo.AddManualQuizChecking(submission, courseId, slideId, userId).ConfigureAwait(false);
					await visitsRepo.MarkVisitsAsWithManualChecking(courseId, slideId, userId).ConfigureAwait(false);
				}
			}
			/* Recalculate score for quiz if this attempt is allowed. Don't recalculate score if this attempt number is more then maxTriesCount */
			else if (tryIndex < maxTriesCount)
			{
				var score = allQuizInfos
					.DistinctBy(forDb => forDb.BlockId)
					.Sum(forDb => forDb.QuizBlockScore);
				
				metricSender.SendCount($"quiz.submit.try.{tryIndex}.score", score);
				metricSender.SendCount($"quiz.submit.{courseId}.try.{tryIndex}.score", score);
				metricSender.SendCount($"quiz.submit.{courseId}.{slideId}.try.{tryIndex}.score", score);
				metricSender.SendCount($"quiz.submit.score", score);
				metricSender.SendCount($"quiz.submit.{courseId}.score", score);
				metricSender.SendCount($"quiz.submit.{courseId}.{slideId}.score", score);

				if (score == slide.MaxScore)
				{
					metricSender.SendCount($"quiz.submit.try.{tryIndex}.full_passed");
					metricSender.SendCount($"quiz.submit.{courseId}.try.{tryIndex}.full_passed");
					metricSender.SendCount($"quiz.submit.{courseId}.{slideId}.try.{tryIndex}.full_passed");
					metricSender.SendCount($"quiz.submit.full_passed");
					metricSender.SendCount($"quiz.submit.{courseId}.full_passed");
					metricSender.SendCount($"quiz.submit.{courseId}.{slideId}.full_passed");
				}
				
				await slideCheckingsRepo.AddAutomaticQuizChecking(submission, courseId, slideId, userId, score).ConfigureAwait(false);
				await visitsRepo.UpdateScoreForVisit(courseId, slideId, userId).ConfigureAwait(false);
				if (isLti)
					LtiUtils.SubmitScore(courseId, slide, userId);
			}

			return Json(new
			{
				url = Url.RouteUrl("Course.SlideById", new { courseId = courseId, slideId = slide.Url, send = 1}) 
			});
		}

		private async Task NotifyAboutManualQuizChecking(ManualQuizChecking checking)
		{
			var notification = new PassedManualQuizCheckingNotification
			{
				Checking = checking,
			};
			await notificationsRepo.AddNotification(checking.CourseId, notification, User.Identity.GetUserId()).ConfigureAwait(false);
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> ScoreQuiz(int id, string nextUrl, string errorUrl = "")
		{
			metricSender.SendCount("quiz.manual_score");

			if (string.IsNullOrEmpty(errorUrl))
				errorUrl = nextUrl;

			using (var transaction = db.Database.BeginTransaction())
			{
				var checking = slideCheckingsRepo.FindManualCheckingById<ManualQuizChecking>(id);

				if (checking.IsChecked)
					return Redirect(errorUrl + "Эта работа уже была проверена");

				if (!checking.IsLockedBy(User.Identity))
					return Redirect(errorUrl + "Эта работа проверяется другим инструктором");

				var course = courseManager.GetCourse(checking.CourseId);
				var unit = course.FindUnitBySlideId(checking.SlideId);

				metricSender.SendCount($"quiz.manual_score.{checking.CourseId}");
				metricSender.SendCount($"quiz.manual_score.{checking.CourseId}.{checking.SlideId}");

				var totalScore = 0;

				var quiz = course.FindSlideById(checking.SlideId);
				if (quiz == null)
					return Redirect(errorUrl + "Этого теста больше нет в курсе");
						
				foreach (var question in quiz.Blocks.OfType<AbstractQuestionBlock>())
				{
					var scoreFieldName = "quiz__score__" + question.Id;
					var scoreStr = Request.Form[scoreFieldName];
					/* Invalid form: score isn't integer */
					if (!int.TryParse(scoreStr, out var score))
						return Redirect(errorUrl + $"Неверное количество баллов в задании «{question.QuestionIndex}. {question.Text.TruncateWithEllipsis(50)}»");
					/* Invalid form: score isn't from range 0..MAX_SCORE */
					if (score < 0 || score > question.MaxScore)
						return Redirect(errorUrl + $"Неверное количество баллов в задании «{question.QuestionIndex}. {question.Text.TruncateWithEllipsis(50)}»: {score}");

					await userQuizzesRepo.SetScoreForQuizBlock(checking.Submission.Id, question.Id, score).ConfigureAwait(false);
					totalScore += score;
				}

				await slideCheckingsRepo.MarkManualCheckingAsChecked(checking, totalScore).ConfigureAwait(false);
				await visitsRepo.UpdateScoreForVisit(checking.CourseId, checking.SlideId, checking.UserId).ConfigureAwait(false);
				transaction.Commit();

				metricSender.SendCount($"quiz.manual_score.score", totalScore);
				metricSender.SendCount($"quiz.manual_score.{checking.CourseId}.score", totalScore);
				metricSender.SendCount($"quiz.manual_score.{checking.CourseId}.{checking.SlideId}.score", totalScore);
				if (totalScore == quiz.MaxScore)
				{
					metricSender.SendCount($"quiz.manual_score.full_scored");
					metricSender.SendCount($"quiz.manual_score.{checking.CourseId}.full_scored");
					metricSender.SendCount($"quiz.manual_score.{checking.CourseId}.{checking.SlideId}.full_scored");
				}

				await NotifyAboutManualQuizChecking(checking);
			}

			return Redirect(nextUrl);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfo(QuizSlide slide, IGrouping<string, QuizAnswer> answer)
		{
			var block = slide.FindBlockById(answer.Key);
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, answer.First().Text);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer);
			if (block is OrderingBlock)
				return CreateQuizInfoForDb(block as OrderingBlock, answer);
			if (block is MatchingBlock)
				return CreateQuizInfoForDb(block as MatchingBlock, answer);
			if (block is IsTrueBlock)
				return CreateQuizInfoForDb(block as IsTrueBlock, answer);
			return null;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IGrouping<string, QuizAnswer> data)
		{
			var isTrue = isTrueBlock.IsRight(data.First().ItemId);
			var blockScore = isTrue ? isTrueBlock.MaxScore : 0;
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					BlockId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrue,
					Text = data.First().ItemId,
					BlockType = typeof(IsTrueBlock),
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
				var isCorrect = choiceBlock.Items.First(x => x.Id == answerItemId).IsCorrect.IsTrueOrMaybe();
				blockScore = isCorrect ? choiceBlock.MaxScore : 0;
				return new List<QuizInfoForDb>
				{
					new QuizInfoForDb
					{
						BlockId = choiceBlock.Id,
						ItemId = answerItemId,
						IsRightAnswer = isCorrect,
						Text = null,
						BlockType = typeof(ChoiceBlock),
						QuizBlockScore = blockScore,
						QuizBlockMaxScore = choiceBlock.MaxScore
					}
				};
			}
			var ans = answers.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					BlockId = choiceBlock.Id,
					IsRightAnswer = choiceBlock.Items.Where(y => y.IsCorrect.IsTrueOrMaybe()).Any(y => y.Id == x),
					ItemId = x,
					Text = null,
					BlockType = typeof(ChoiceBlock),
					QuizBlockScore = 0,
					QuizBlockMaxScore = choiceBlock.MaxScore
				}).ToList();
			
			var mistakesCount = GetChoiceBlockMistakesCount(choiceBlock, ans);
			var isRightQuizBlock = mistakesCount.HasNotMoreThatAllowed(choiceBlock.AllowedMistakesCount);
			
			blockScore = isRightQuizBlock ? choiceBlock.MaxScore : 0;
			foreach (var info in ans)
				info.QuizBlockScore = blockScore;
			return ans;
		}

		private MistakesCount GetChoiceBlockMistakesCount(ChoiceBlock choiceBlock, List<QuizInfoForDb> ans)
		{
			var checkedUnnecessary = ans.Count(x => !x.IsRightAnswer);
			
			var totallyTrueItemIds = choiceBlock.Items.Where(x => x.IsCorrect == ChoiceItemCorrectness.True).Select(x => x.Id);
			var userItemIds = ans.Select(y => y.ItemId).ToImmutableHashSet();
			var notCheckedNecessary = totallyTrueItemIds.Count(x => ! userItemIds.Contains(x));
			
			return new MistakesCount(checkedUnnecessary, notCheckedNecessary);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(OrderingBlock orderingBlock, IGrouping<string, QuizAnswer> answers)
		{
			var ans = answers.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					BlockId = orderingBlock.Id,
					IsRightAnswer = true,
					ItemId = x,
					Text = null,
					BlockType = typeof(OrderingBlock),
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
					BlockId = matchingBlock.Id,
					IsRightAnswer = matchingBlock.Matches.FirstOrDefault(m => m.GetHashForFixedItem() == x.ItemId)?.
										GetHashForMovableItem() == x.Text,
					ItemId = x.ItemId,
					Text = x.Text,
					BlockType = typeof(MatchingBlock),
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
			if (data.Length > MaxFillInBlockSize)
				data = data.Substring(0, MaxFillInBlockSize);
			var isRightAnswer = true;
			if (fillInBlock.Regexes != null)
				isRightAnswer = fillInBlock.Regexes.Any(regex => regex.Regex.IsMatch(data));
			var blockScore = isRightAnswer ? fillInBlock.MaxScore : 0;
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					BlockId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = isRightAnswer,
					Text = data,
					BlockType = typeof(FillInBlock),
					QuizBlockScore = blockScore,
					QuizBlockMaxScore = fillInBlock.MaxScore
				}
			};
		}

		private static IEnumerable<QuizAnswerInfo> GetUserQuizAnswers(QuizSlide slide, IEnumerable<UserQuizAnswer> userQuizzes)
		{
			var answers = userQuizzes.GroupBy(q => q.BlockId).ToDictionary(g => g.Key, g => g.ToList());
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

		private static QuizAnswerInfo GetFillInBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuizAnswer>> answers, string quizId, int questionIndex)
		{
			UserQuizAnswer answer = null;
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

		private static QuizAnswerInfo GetChoiceBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuizAnswer>> answers, ChoiceBlock block, int questionIndex)
		{
			IEnumerable<UserQuizAnswer> answer = new List<UserQuizAnswer>();
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
				CorrectAnswer = new HashSet<string>(block.Items.Where(x => x.IsCorrect.IsTrueOrMaybe()).Select(x => x.Id)),
				Score = score,
				MaxScore = maxScore,
				IsRight = isRight
			};
		}


		private static QuizAnswerInfo GetIsTrueBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuizAnswer>> answers, string quizId, int questionIndex)
		{
			UserQuizAnswer answer = null;
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

		private static QuizAnswerInfo GetOrderingBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuizAnswer>> answers, OrderingBlock block, int questionIndex)
		{
			IEnumerable<UserQuizAnswer> userAnswers = new List<UserQuizAnswer>();
			UserQuizAnswer firstAnswer = null;
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

		private static QuizAnswerInfo GetMatchingBlockAnswerInfo(IReadOnlyDictionary<string, List<UserQuizAnswer>> answers, MatchingBlock block, int questionIndex)
		{
			IEnumerable<UserQuizAnswer> userAnswers = new List<UserQuizAnswer>();
			UserQuizAnswer firstAnswer = null;
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
				var userQuizDrops = userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).Count(b => b);
				var maxTriesCount = GetMaxTriesCount(courseId, slide as QuizSlide);
				var isQuizScoredMaximum = userQuizzesRepo.IsQuizScoredMaximum(courseId, slideId, userId);
				if (userQuizDrops + 1 < maxTriesCount && !isQuizScoredMaximum)
				{
					await userQuizzesRepo.DropQuizAsync(courseId, slideId, userId);
					await visitsRepo.UpdateScoreForVisit(courseId, slideId, userId);
					if (isLti)
						LtiUtils.SubmitScore(courseId, slide, userId);
				}
				else if ((userQuizDrops + 1 >= maxTriesCount || isQuizScoredMaximum) && !(slide as QuizSlide).ManualChecking)
				{
					/* Allow user to drop quiz after all tries are exceeded, but don't update score */
					await userQuizzesRepo.DropQuizAsync(courseId, slideId, userId);
				}
				
			}
			var model = new { courseId, slideId = slide.Id, isLti };
			if (isLti)
				return RedirectToAction("LtiSlide", "Course", model);
			return RedirectToAction("SlideById", "Course", model);
		}

		private QuizModel GuestQuiz(Course course, QuizSlide slide)
		{
			return new QuizModel
			{
				Course = course,
				Slide = slide,
				IsGuest = true,
				QuizState = QuizState.NotPassed,
			};
		}

		private int GetMaxTriesCount(string courseId, QuizSlide quizSlide)
		{
			if (User.HasAccessFor(courseId, CourseRole.Tester))
				return InfinityTriesCount;

			if (quizSlide == null)
				return defaultMaxTriesCount;
			
			return quizSlide.MaxTriesCount;
		}

		private Dictionary<string, int> GetScoresForBlocks(string courseId, string userId, Guid slideId, QuizState state)
		{
			return userQuizzesRepo.GetUserScores(courseId, slideId, userId);
		}

		private Tuple<QuizState, int> GetQuizState(string courseId, string userId, Guid slideId)
		{
			log.Info($"Ищу статус прохождения теста {courseId}:{slideId} для пользователя {userId}");
			var states = userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).ToList();

			var submission = userQuizzesRepo.FindLastUserSubmission(courseId, slideId, userId);
			var queueItem = submission?.ManualChecking;
			if (queueItem != null)
			{
				log.Info($"Статус прохождения теста {courseId}:{slideId} для пользователя {userId}: есть ручная проверка №{queueItem.Id}, проверяется ли сейчас: {queueItem.IsLocked}");
				if (queueItem.IsChecked)
					return Tuple.Create(QuizState.Subtotal, states.Count);
				return Tuple.Create(queueItem.IsLocked ? QuizState.IsChecking : QuizState.WaitForCheck, states.Count);
			}

			if (states.Any(b => !b))
				return Tuple.Create(QuizState.Subtotal, states.Count);
			return Tuple.Create(QuizState.NotPassed, states.Count);
		}
	}
}