using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
	public class SandboxController : Controller
	{
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		private static readonly TimeSpan timeout = TimeSpan.FromSeconds(30);

		public ActionResult Index(int max = 200, int skip = 0)
		{
			var submissions = solutionsRepo.GetAllSubmissions(max, skip).ToList();
			return View(new SubmissionsListModel
			{
				Submissions = submissions
			});
		}

		public ActionResult RunCode()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Run()
		{
			var code = Request.InputStream.GetString();

			var automaticExerciseChecking = await solutionsRepo.RunUserSolution(
				"web", Guid.Empty, User.Identity.GetUserId(),
				code, null, null, false, "null",
				User.Identity.Name + ": CsSandbox Web Executor", timeout
				);

			if (automaticExerciseChecking == null)
			{
				return Json(new RunSolutionResult
				{
					CompilationError = "Что-то пошло не так :(",
					IsCompileError = true,
				});
			}

			return Json(new RunSolutionResult
			{
				ActualOutput = automaticExerciseChecking.Output.Text,
				CompilationError = automaticExerciseChecking.CompilationError.Text,
				ExecutionServiceName = automaticExerciseChecking.ExecutionServiceName,
				IsCompileError = automaticExerciseChecking.IsCompilationError,
				IsRightAnswer = automaticExerciseChecking.IsRightAnswer
			});
		}

		public ActionResult GetDetails(int id)
		{
			var submission = solutionsRepo.GetSubmission(id);

			submission.SolutionCode.Text = ((ExerciseSlide)courseManager
				.GetCourse(submission.CourseId)
				.GetSlideById(submission.SlideId))
				.Exercise
				.GetSourceCode(submission.SolutionCode.Text);

			return View(submission);
		}
	}
}