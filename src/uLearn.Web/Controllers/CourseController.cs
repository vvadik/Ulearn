using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
		private readonly ExecutionService executionService = new ExecutionService();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();

		public CourseController() : this(CourseManager.AllCourses)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}
		
		[Authorize]
		public ActionResult Slide(string courseId, int slideIndex = 0)
		{
			var model = CreateCoursePageModel(courseId, slideIndex);
			return View(model);
		}

		private CoursePageModel CreateCoursePageModel(string courseId, int slideIndex)
		{
			Course course = courseManager.GetCourse(courseId);
			var isPassedTask = solutionsRepo.IsUserPassedTask(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId());
			var model = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex],
				NextSlideIndex = slideIndex + 1,
				PrevSlideIndex = slideIndex - 1,
				IsPassedTask = isPassedTask,
				LatestAcceptedSolution = isPassedTask ? solutionsRepo.GetLatestAcceptedSolution(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId()) : null
			};
			return model;
		}

		[Authorize]
		public ActionResult AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var coursePageModel = CreateCoursePageModel(courseId, slideIndex);
			var model = new AcceptedSolutionsPageModel
			{
				CoursePageModel = coursePageModel,
				AcceptedSolutions = coursePageModel.IsPassedTask ? solutionsRepo.GetAllAcceptedSolutions(courseId, coursePageModel.Course.Slides[slideIndex].Id) : new List<AcceptedSolutionInfo>()
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var result = await CheckSolution(exerciseSlide, code, slideIndex);
			await SaveUserSolution(courseId, exerciseSlide.Id, code, result.CompilationError, result.ActualOutput, result.IsRightAnswer);
			return Json(result);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> AddQuestion(string title, string unitName)
		{
			var question = GetUserCode(Request.InputStream);
			var userName = User.Identity.GetUserName();
			await userQuestionsRepo.AddUserQuestion(question, title, userName,unitName, DateTime.Now);
			return "Success!";
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyMark(string courseId, string slideId, string rate )
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates)Enum.Parse(typeof(SlideRates), rate);
			await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
			return "success!";
		}

		[HttpPost]
		[Authorize]
		public string GetMark(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			return slideRateRepo.FindRate(courseId, slideId, userId);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> AddHint(string courseId, string slideId, int hintId)
		{
			var userId = User.Identity.GetUserId();
			await slideHintRepo.AddHint(userId, hintId, courseId, slideId);
			return "success";
		}

		[HttpPost]
		[Authorize]
		public string GetHint(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			var answer = slideHintRepo.GetHint(userId, courseId, slideId);
			return answer;
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
		public async Task<string> LikeSolution()
		{
			var id = GetUserCode(Request.InputStream);
			var like = await solutionsRepo.Like(int.Parse(id), User.Identity.GetUserId());
			return like;
		}

		private string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		private async Task<RunSolutionResult> CheckSolution(ExerciseSlide exerciseSlide, string code, int slideIndex)
		{
			var solution = exerciseSlide.Solution.BuildSolution(code);
			var submition = await executionService.Submit(solution, "");
			var isRightAnswer = NormalizeString(submition.Output).Equals(NormalizeString(exerciseSlide.ExpectedOutput));
			return new RunSolutionResult
			{
				CompilationError = submition.CompilationError,
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseSlide.ExpectedOutput,
				ActualOutput = submition.Output
			};
		}

		private async Task SaveUserSolution(string courseId, string slideId, string code, string compilationError, string output,
			bool isRightAnswer)
		{
			await solutionsRepo.AddUserSolution(
				courseId, slideId, 
				code, isRightAnswer, compilationError, output,
				User.Identity.GetUserId());
		}


		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
		}
	}
}