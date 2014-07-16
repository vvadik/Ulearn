using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly ExecutionService executionService = new ExecutionService();

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
			Course course = courseManager.GetCourse(courseId);
			var model = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex]
			};
			return View(model);
		}

		[Authorize]
		public ActionResult AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			Course course = courseManager.GetCourse(courseId);
			var coursePageModel = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex]
			};
			var isLegal = solutionsRepo.IsUserPassedTask(courseId, slideIndex, User.Identity.GetUserId());
			var model = new AcceptedSolutionsPageModel
			{
				CoursePageModel = coursePageModel,
				AcceptedSolutions = isLegal ? solutionsRepo.GetAllAcceptedSolutions(slideIndex) : new List<AcceptedSolutionInfo>()
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
			await SaveUserSolution(courseId, slideIndex, code, result.ExecutionResult, result.IsRightAnswer);
			return Json(result);
		}

		[HttpPost]
		[Authorize]
		public async Task<Like> LikeSolution()
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
				ExecutionResult = submition,
				IsRightAnswer = isRightAnswer
			};
		}

		private async Task SaveUserSolution(string courseId, int slideIndex, string code, GetSubmitionDetailsResult submition,
			bool isRightAnswer)
		{
			await solutionsRepo.AddUserSolution(
				courseId, slideIndex, 
				code, isRightAnswer, submition.CompilationError, submition.Output,
				User.Identity.GetUserId());
		}


		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}
	}
}