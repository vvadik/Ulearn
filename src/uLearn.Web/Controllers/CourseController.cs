using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UserQuestionsRepo userQuestionsRepo = new UserQuestionsRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

		public CourseController()
			: this(CourseManager.AllCourses)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public async Task<ActionResult> Slide(string courseId, int slideIndex = 0)
		{
			var model = await CreateCoursePageModel(courseId, slideIndex);
			var exerciseSlide = model.Slide as ExerciseSlide;
			if (exerciseSlide != null)
				exerciseSlide.LikedHints = slideHintRepo.GetLikedHints(courseId, exerciseSlide.Id, User.Identity.GetUserId());
			return View(model);
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, int slideIndex)
		{
			Course course = courseManager.GetCourse(courseId);
			var isPassedTask = solutionsRepo.IsUserPassedTask(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId());
			await VisitSlide(courseId, course.Slides[slideIndex].Id);
			var model = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex],
				NextSlideIndex = slideIndex + 1,
				PrevSlideIndex = slideIndex - 1,
				IsPassedTask = isPassedTask,
				LatestAcceptedSolution = isPassedTask ? solutionsRepo.GetLatestAcceptedSolution(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId()) : null,
				Rate = GetRate(course.Id, course.Slides[slideIndex].Id),
				SolvedSlide = solutionsRepo.GetIdOfPassedSlides(course.Id, User.Identity.GetUserId()),
				VisitedSlide = visitersRepo.GetIdOfVisitedSlides(course.Id, User.Identity.GetUserId()),
				PassedQuiz = userQuizzesRepo.GetIdOfQuizPassedSlides(courseId, User.Identity.GetUserId()),
				AnswersToQuizes = userQuizzesRepo.GetAnswersForShowOnSlide(courseId, course.Slides[slideIndex] as QuizSlide, User.Identity.GetUserId())
			};
			return model;
		}

		[Authorize]
		public async Task<ActionResult> AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var coursePageModel = await CreateCoursePageModel(courseId, slideIndex);
			coursePageModel.PrevSlideIndex = coursePageModel.PrevSlideIndex + 1;
			coursePageModel.IsSlideWithAcceptedSolutions = true;
			var solutions = coursePageModel.IsPassedTask
				? solutionsRepo.GetAllAcceptedSolutions(courseId, coursePageModel.Course.Slides[slideIndex].Id)
				: new List<AcceptedSolutionInfo>();
			foreach (var solution in solutions)
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);

			var model = new AcceptedSolutionsPageModel
			{
				CoursePageModel = coursePageModel,
				AcceptedSolutions = solutions
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> AddQuestion(string title, string unitName, string question)
		{
			var userName = User.Identity.GetUserName();
			await userQuestionsRepo.AddUserQuestion(question, title, userName, unitName, DateTime.Now);
			return "Success!";
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyRate(string courseId, string slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates)Enum.Parse(typeof(SlideRates), rate);
			return await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
		}

		[HttpPost]
		[Authorize]
		public string GetRate(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			return slideRateRepo.FindRate(courseId, slideId, userId);
		}

		[HttpPost]
		[Authorize]
		public string GetAllQuestions(string courseName)
		{
			var questions = userQuestionsRepo.GetAllQuestions(courseName);
			return Server.HtmlEncode(questions);
		}

		[HttpPost]
		[Authorize]
		public async Task<JsonResult> LikeSolution(int solutionId)
		{
			var res = await solutionsRepo.Like(solutionId, User.Identity.GetUserId());
			return Json(new { likesCount = res.Item1, liked = res.Item2 });
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
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
			var answers = answer.Split('*').Select(x => x.Split('_').Take(2).ToList()).GroupBy(x => x[0]);
			var quizBlockWithTaskCount = (course.Slides[intSlideIndex] as QuizSlide).Quiz.Blocks.Count(x => x is FillInBlock || x is IsTrueBlock || x is ChoiceBlock);
			var incorrectQuizzes = new List<string>();
			var fillInBlockType = typeof (FillInBlock);
			var tmpFolder = new List<QuizInfoForDb>();
			foreach (var ans in answers)
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
				return "has empty bloks";
			foreach (var quizInfoForDb in tmpFolder)
				await userQuizzesRepo.AddUserQuiz(courseId, quizInfoForDb.IsRightAnswer, quizInfoForDb.ItemId, quizInfoForDb.QuizId,
					course.Slides[intSlideIndex].Id, quizInfoForDb.Text, User.Identity.GetUserId(), time);
			return string.Join("*", incorrectQuizzes.Distinct());
		}

		[HttpPost]
		public string GetRightAnswersToQuiz(string courseId, int slideIndex)
		{
			var course = courseManager.GetCourse(courseId);
			var quizSlide = course.Slides[slideIndex];
			var e = (quizSlide as QuizSlide).RightAnswersToQuiz;
			return e;
		}

		private IEnumerable<QuizInfoForDb> GetQuizInfo(Course course, int slideIndex, IGrouping<string, List<string>> answer)
		{
			var slide = course.Slides[slideIndex] as QuizSlide;
			if (slide == null)
				throw new Exception("Error in func 'GetSlideUsingId' or in 'QuizSlideId'");
			var block = slide.GetBlockById(answer.Key);
			var data = answer.ToList();
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, data);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer, data);
			return CreateQuizInfoForDb(block as IsTrueBlock, data);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IEnumerable<List<string>> data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrueBlock.Answer.ToString() == data.First()[1],
					Text = data.First()[1],
					QuizType = typeof(IsTrueBlock)
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(ChoiceBlock choiseBlock, IEnumerable<List<string>> answer, List<List<string>> data)
		{
			if (!choiseBlock.Multiple)
				return new List<QuizInfoForDb>
				{
					new QuizInfoForDb
					{
						QuizId = choiseBlock.Id,
						ItemId = data.First()[1],
						IsRightAnswer = choiseBlock.Items.First(x => x.Id == data.First()[1]).IsCorrect,
						Text = null,
						QuizType = typeof (ChoiceBlock)
					}
				};
			var ans = answer.ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = choiseBlock.Id, 
					IsRightAnswer = false, 
					ItemId = x[1], 
					Text = null, 
					QuizType = typeof(ChoiceBlock)
					
				}).ToList();
			var correctItems = new HashSet<string>(choiseBlock.Items.Where(x => x.IsCorrect).Select(x => x.Id));
			var ansItem = new HashSet<string>(ans.Select(x => x.ItemId));
			var count = ansItem.Count(correctItems.Contains);
			if (count != correctItems.Count || count != ansItem.Count) return ans;
			foreach (var info in ans)
				info.IsRightAnswer = true;
			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(FillInBlock fillInBlock, IEnumerable<List<string>> data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = fillInBlock.Regexes.Any(regex => Regex.IsMatch(data.First()[1], regex)),
					Text = data.First()[1],
					QuizType = typeof(FillInBlock)
				}
			};
		}
	}

	public class QuizInfoForDb
	{
		public string QuizId;
		public string ItemId;
		public string Text;
		public bool IsRightAnswer;
		public Type QuizType;
	}
}