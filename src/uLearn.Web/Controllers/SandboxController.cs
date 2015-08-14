using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class SandboxController : Controller
	{
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		private const int _timeout = 30;

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult Index(int max = 200, int skip = 0)
		{
			var submissions = solutionsRepo
				.GetAllSolutions(max, skip).ToList();
			return View(new SubmissionsListModel
			{
				Submissions = submissions
			}); 
		}

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult RunCode()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public async Task<ActionResult> Run()
		{
			var token = Request.Cookies["token"];
			if (token == null) return Index();
			var code = Request.InputStream.GetString();

			var solution = await solutionsRepo.RunUserSolution(
				"web", "runner", User.Identity.GetUserId(), 
				code, null, null, false, "null", 
				User.Identity.Name + ": CsSandbox Web Executor", _timeout
			);

			return Json(new RunSolutionResult
			{
				ActualOutput = solution.Output.Text, 
				CompilationError = solution.CompilationError.Text, 
				ExecutionServiceName = solution.ExecutionServiceName, 
				IsCompileError = solution.IsCompilationError, 
				IsRightAnswer = solution.IsRightAnswer
			});
		}

		[Authorize(Roles = LmsRoles.Admin + "," + LmsRoles.Instructor)]
		public ActionResult GetDetails(int id)
		{
			var details = solutionsRepo.GetDetails(id);
			details.SolutionCode.Text = Utils.GetSource(
				details.CourseId, 
				details.SlideId, 
				courseManager, 
				details.SolutionCode.Text);
			return View(solutionsRepo.GetDetails(id));
		}
	}
}