using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();

		public CourseController() : this(CourseManager.AllCourses)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

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

		[HttpPost]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			Course course = courseManager.GetCourse(courseId);
			var exerciseSlide = course.Slides[slideIndex] as ExerciseSlide;
			if (exerciseSlide == null) return Json("not a exercise");
			var solution = exerciseSlide.Solution.BuildSolution(code);
			GetSubmitionDetailsResult submition = await new ExecutionService().Submit(solution, "");
			var isRightAnswer = submition.Output.Trim().Equals(exerciseSlide.ExpectedOutput.Trim());
//			ApplicationUser user = Request.IsAuthenticated ? User.
//			db.UserSolutions.Add(new UserSolution
//			{
//				Code = code,
//				CompilationError = submition.CompilationError,
//				CourseId = courseId,
//				SlideId = slideIndex.ToString(),
//				IsCompilationError = !string.IsNullOrWhiteSpace(submition.CompilationError),
//				IsRightAnswer = isRightAnswer,
//				Output = submition.Output,
//				Timestamp = DateTime.Now,
////				User = db.Users.User.Identity.Name
//			});
			return Json(
				new RunSolutionResult { 
					ExecutionResult = submition, 
					IsRightAnswer = isRightAnswer});
		}

		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}
	}
}