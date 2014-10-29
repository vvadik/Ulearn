using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class QuizController : Controller
	{
		private const int MAX_DROPS_COUNT = 1;

		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

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
			public bool IsRightQuizBlock;
		}

		[HttpPost]
		[Authorize(Roles = LmsRoles.Tester)]
		public async Task<ActionResult> ClearAnswers(string courseId, string slideId)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			await userQuizzesRepo.RemoveAnswers(User.Identity.GetUserId(), slideId);
			return RedirectToAction("Slide", "Course", new { courseId, slideIndex = slide.Index });
		}

		[HttpPost]
		[Authorize]
		public async Task<string> SubmitQuiz(string courseId, string slideIndex, string answer)
		{
			var intSlideIndex = int.Parse(slideIndex);
			var course = courseManager.GetCourse(courseId);
			if (userQuizzesRepo.IsQuizSlidePassed(courseId, User.Identity.GetUserId(), course.Slides[intSlideIndex].Id))
				return "already answered";
			var time = DateTime.Now;
			var quizzes = JsonConvert.DeserializeObject<List<QuizAnswer>>(answer).GroupBy(x => x.QuizId);
			var quizBlockWithTaskCount = ((QuizSlide)course.Slides[intSlideIndex]).Quiz.Blocks.Count(x => x is FillInBlock || x is IsTrueBlock || x is ChoiceBlock);
			var incorrectQuizzes = new List<string>();
			var fillInBlockType = typeof(FillInBlock);
			var tmpFolder = new List<QuizInfoForDb>();
			foreach (var ans in quizzes)
			{
				var quizInfo = GetQuizInfo(course, intSlideIndex, ans);
				foreach (var quizInfoForDb in quizInfo)
				{
					tmpFolder.Add(quizInfoForDb);
					if (!quizInfoForDb.IsRightAnswer && quizInfoForDb.QuizType == fillInBlockType)
						incorrectQuizzes.Add(ans.Key);
				}
			}
			var blocksInAnswerCount = tmpFolder.Select(x => x.QuizId).Distinct().Count();
			if (blocksInAnswerCount != quizBlockWithTaskCount)
				return "has empty blocks";
			foreach (var quizInfoForDb in tmpFolder)
				await userQuizzesRepo.AddUserQuiz(courseId, quizInfoForDb.IsRightAnswer, quizInfoForDb.ItemId, quizInfoForDb.QuizId,
					course.Slides[intSlideIndex].Id, quizInfoForDb.Text, User.Identity.GetUserId(), time, quizInfoForDb.IsRightQuizBlock);
			return string.Join("*", incorrectQuizzes.Distinct());
		}

		private IEnumerable<QuizInfoForDb> GetQuizInfo(Course course, int slideIndex, IGrouping<string, QuizAnswer> answer)
		{
			var slide = course.Slides[slideIndex] as QuizSlide;
			if (slide == null)
				throw new Exception("Error in func 'GetSlideById' or in 'QuizSlideId'");
			var block = slide.GetBlockById(answer.Key);
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, answer.First().Text);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer);
			return CreateQuizInfoForDb(block as IsTrueBlock, answer);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IGrouping<string, QuizAnswer> data)
		{
			var isTrue = isTrueBlock.IsRight(data.First().ItemId);
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrue,
					Text = data.First().ItemId,
					QuizType = typeof(IsTrueBlock),
					IsRightQuizBlock = isTrue
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(ChoiceBlock choiseBlock, IGrouping<string, QuizAnswer> data)
		{
			if (!choiseBlock.Multiple)
			{
				var answerItemId = data.First().ItemId;
				var isTrue = choiseBlock.Items.First(x => x.Id == answerItemId).IsCorrect;
				return new List<QuizInfoForDb>
				{
					new QuizInfoForDb
					{
						QuizId = choiseBlock.Id,
						ItemId = answerItemId,
						IsRightAnswer = isTrue,
						Text = null,
						QuizType = typeof (ChoiceBlock),
						IsRightQuizBlock = isTrue
					}
				};
			}
			var ans = data.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = choiseBlock.Id,
					IsRightAnswer = choiseBlock.Items.Where(y => y.IsCorrect).Any(y => y.Id == x),
					ItemId = x,
					Text = null,
					QuizType = typeof(ChoiceBlock),
					IsRightQuizBlock = false
				}).ToList();
			var isRightQuizBlock = ans.All(x => x.IsRightAnswer) &&
						 choiseBlock.Items.Where(x => x.IsCorrect)
							 .Select(x => x.Id)
							 .All(x => ans.Where(y => y.IsRightAnswer).Select(y => y.ItemId).Contains(x));
			foreach (var info in ans)
				info.IsRightQuizBlock = isRightQuizBlock;
			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(FillInBlock fillInBlock, string data)
		{
			var isTrue = fillInBlock.Regexes.Any(regex => regex.Regex.IsMatch(data));
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrue,
					Text = data,
					QuizType = typeof(FillInBlock),
					IsRightQuizBlock = isTrue
				}
			};
		}

		[HttpGet]
		[Authorize(Roles = LmsRoles.Instructor)]
		public ActionResult Analytics(string courseId, int slideIndex)
		{
			var course = courseManager.GetCourse(courseId);
			var quizSlide = (QuizSlide)course.Slides[slideIndex];
			var dict = new SortedDictionary<string, List<QuizAnswerInfo>>();
			var groups = new Dictionary<string, string>();
			var passedUsers = db.UserQuizzes
				.Where(q => quizSlide.Id == q.SlideId && !q.isDropped)
				.Select(q => q.UserId)
				.Distinct()
				.Join(db.Users, userId => userId, u => u.Id, (userId, u) => new { UserId = userId, u.UserName, u.GroupName });
			foreach (var user in passedUsers)
			{
				dict[user.UserName] = GetUserQuizAnswers(courseId, quizSlide, user.UserId).ToList();
				groups[user.UserName] = user.GroupName;
			}
			var rightAnswersCount = dict.Values
				.SelectMany(list => list
					.Where(info => info.IsRight)
					.Select(info => new { info.Id, info.IsRight }))
				.GroupBy(arg => arg.Id)
				.ToDictionary(grouping => grouping.Key, grouping => grouping.Count());
			return PartialView(new QuizAnalyticsModel
			{
				UserAnswers = dict,
				QuizSlide = quizSlide,
				RightAnswersCount = rightAnswersCount,
				Group = groups
			});
		}

		private IEnumerable<QuizAnswerInfo> GetUserQuizAnswers(string courseId, QuizSlide slide, string userId)
		{
			foreach (var block in slide.Quiz.Blocks.OfType<AbstractQuestionBlock>())
				if (block is FillInBlock)
					yield return userQuizzesRepo.GetFillInBlockAnswerInfo(courseId, slide.Id, block.Id, userId, block.QuestionIndex);
				else if (block is ChoiceBlock)
					yield return userQuizzesRepo.GetChoiseBlockAnswerInfo(courseId, slide.Id, (ChoiceBlock)block, userId, block.QuestionIndex);
				else if (block is IsTrueBlock)
					yield return userQuizzesRepo.GetIsTrueBlockAnswerInfo(courseId, slide.Id, block.Id, userId, block.QuestionIndex);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> DropQuiz(string courseId, string slideId)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			if (slide is QuizSlide)
			{
				var userId = User.Identity.GetUserId();
				if (userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).Count(b => b) < GetMaxDropCount(slide as QuizSlide) &&
				    !userQuizzesRepo.GetQuizBlocksTruth(courseId, userId, slideId).All(b => b.Value))
					await userQuizzesRepo.DropQuiz(userId, slideId);
			}
			return RedirectToAction("Slide", "Course", new { courseId, slideIndex = slide.Index });
		}

		public ActionResult Quiz(QuizSlide slide, string courseId, string userId)
		{
			var slideId = slide.Id;
			var maxDropCount = GetMaxDropCount(slide);
			var state = GetQuizState(courseId, userId, slideId, maxDropCount);
			var resultsForQuizes = GetResultForQuizes(courseId, userId, slideId, state.Item1);
			var model = new QuizModel
			{
				CourseId = courseId,
				Slide = slide,
				QuizState = state.Item1,
				TryNumber = state.Item2,
				MaxDropCount = maxDropCount,
				ResultsForQuizes = resultsForQuizes,
				AnswersToQuizes =
					userQuizzesRepo.GetAnswersForShowOnSlide(courseId, slide,
						userId)
			};

			if (model.QuizState != QuizState.NotPassed && model.RightAnswers == model.QuestionsCount)
				model.QuizState = QuizState.Total;

			return PartialView(model);
		}

		private static int GetMaxDropCount(QuizSlide quizSlide)
		{
			if (quizSlide == null)
				return MAX_DROPS_COUNT;
			var maxDropCount = quizSlide.Quiz.MaxDropCount;
			return maxDropCount == 0 ? MAX_DROPS_COUNT : maxDropCount;
		}

		private Dictionary<string, bool> GetResultForQuizes(string courseId, string userId, string slideId, QuizState state)
		{
			return userQuizzesRepo.GetQuizBlocksTruth(courseId, userId, slideId);
		}

		private Tuple<QuizState, int> GetQuizState(string courseId, string userId, string slideId, int maxDropCount)
		{
			var states = userQuizzesRepo.GetQuizDropStates(courseId, userId, slideId).ToList();
			if (states.Count > maxDropCount)
				return Tuple.Create(QuizState.Total, states.Count);
			if (states.Any(b => !b))
				return Tuple.Create(QuizState.Subtotal, states.Count);
			return Tuple.Create(QuizState.NotPassed, states.Count);
		}
	}
}