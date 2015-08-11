using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsSandboxApi;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.ExecutionService;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class SandboxController : Controller
	{
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Run()
		{
			var token = Request.Cookies["token"];
			if (token == null) return Index();
			var code = GetUserCode(Request.InputStream);
			var solution = await SaveUserSolution("web", "runner", code, null, null, false, "null", User.Identity.Name + ": CsSandbox Web Executor");
			return Json(new RunSolutionResult
			{
				ActualOutput = solution.Output.Text, 
				CompilationError = solution.CompilationError.Text, 
				ExecutionServiceName = solution.ExecutionServiceName, 
				IsCompileError = solution.IsCompilationError, 
				IsRightAnswer = solution.IsRightAnswer
			});
		}

		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}

		private async Task<UserSolution> SaveUserSolution(string courseId, string slideId, string code, string compilationError, string output, bool isRightAnswer, string executionServiceName, string displayName)
		{
			var userId = User.Identity.GetUserId();
			var solution = await solutionsRepo.AddUserSolution(
				courseId, slideId,
				code, isRightAnswer, compilationError, output,
				userId, executionServiceName, displayName);

			var count = 30;
			var lastStatus = solution.Status;
			while (lastStatus != SubmissionStatus.Done && count >= 0)
			{
				await Task.Delay(1000);
				--count;
				lastStatus = solutionsRepo.GetDetails(solution.Id.ToString()).Status;
			}
			if (lastStatus != SubmissionStatus.Done)
				return null;

			return solutionsRepo.GetDetails(solution.Id.ToString());
		}

		public ActionResult Submissions(int max = 200, int skip = 0)
		{
			return View(new SubmissionsModel
			{
				Max = max,
				Skip = skip
			});
		}

		public ActionResult SubmissionsList(int max = 200, int skip = 0)
		{
			
			var submissions = solutionsRepo
				.GetAllSolutions(max, skip)
				.Select(details => details);
			return PartialView(new SubmissionsListModel
			{
				Submissions = submissions
			});
		}

		public ActionResult GetDetails(string id)
		{
			var details = solutionsRepo.GetDetails(id);
			details.SolutionCode.Text = Utils.GetSource(details.CourseId, details.SlideId, courseManager, details.SolutionCode.Text);
			return View(solutionsRepo.GetDetails(id));
		}

	}
}