using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;

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
			var codeBytes = new MemoryStream();
			Request.InputStream.CopyTo(codeBytes);
			var code = Encoding.UTF8.GetString(codeBytes.ToArray());
			Course course = courseManager.GetCourse(courseId);
			var exerciseSlide = course.Slides[slideIndex] as ExerciseSlide;
			if (exerciseSlide == null) return Json("not a exercise");
			var solution = exerciseSlide.Solution.BuildSolution(code);
			GetSubmitionDetailsResult submition = await new ExecutionService().Submit(solution, "");
			return Json(
				new RunSolutionResult { 
					ExecutionResult = submition, 
					IsRightAnswer = submition.Output.Trim().Equals(exerciseSlide.ExpectedOutput.Trim())});
		}
	}
}