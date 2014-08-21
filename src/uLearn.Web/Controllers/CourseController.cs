using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using linqed;
using Microsoft.AspNet.Identity;
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
			var quizSlide = model.Slide as QuizSlide;
			if (quizSlide != null)
				foreach (var block in quizSlide.Quiz.Blocks.Where(x => x is ChoiceBlock).Where(x => ((ChoiceBlock)x).Shuffle))
					Shuffle(block);
			return View(model);
		}

		private void Shuffle(QuizBlock quizBlock)
		{
			var random = new Random();
			var choiceBlock = (ChoiceBlock)quizBlock;
			choiceBlock.Items = choiceBlock.Items.OrderBy(x => random.Next()).ToArray();
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, int slideIndex)
		{
			Course course = courseManager.GetCourse(courseId);
			await VisitSlide(courseId, course.Slides[slideIndex].Id);
			var model = new CoursePageModel
			{
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = course.Slides[slideIndex],
				LatestAcceptedSolution =
					solutionsRepo.FindLatestAcceptedSolution(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId()),
				Rate = GetRate(course.Id, course.Slides[slideIndex].Id),
				PassedQuiz = userQuizzesRepo.GetIdOfQuizPassedSlides(courseId, User.Identity.GetUserId()),
				AnswersToQuizes =
					userQuizzesRepo.GetAnswersForShowOnSlide(courseId, course.Slides[slideIndex] as QuizSlide,
						User.Identity.GetUserId())
			};
			return model;
		}

		[Authorize]
		public ActionResult AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = course.Slides[slideIndex];
			var isPassed = solutionsRepo.IsUserPassedTask(courseId, slide.Id, userId);
			var solutions = isPassed
				? solutionsRepo.GetAllAcceptedSolutions(courseId, slide.Id)
				: new List<AcceptedSolutionInfo>();
			foreach (var solution in solutions)
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);
			var model = new AcceptedSolutionsPageModel
			{
				CourseId = courseId,
				CourseTitle = course.Title,
				Slide = slide,
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
			var slideRate = (SlideRates) Enum.Parse(typeof (SlideRates), rate);
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
			return Json(new {likesCount = res.Item1, liked = res.Item2});
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
		}
	}
}