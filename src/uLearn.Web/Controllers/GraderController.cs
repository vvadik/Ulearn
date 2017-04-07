using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class GraderController : BaseExerciseController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(GraderController));

		public GradersRepo gradersRepo;

		public GraderController()
			: this(WebCourseManager.Instance)
		{
		}

		public GraderController(CourseManager courseManager) : base(courseManager)
		{
			gradersRepo = new GradersRepo(db);
		}

		public async Task<ActionResult> Submit(Guid clientId, string courseId, Guid slideId, string solution)
		{
			var client = gradersRepo.FindGraderClient(courseId, clientId);
			if (client == null)
				return Json(new { status = "error", message = "Invalid client id" });

			string code;
			try
			{
				code = Encoding.UTF8.GetString(Convert.FromBase64String(solution));
			}
			catch (DecoderFallbackException e)
			{
				return Json(new { status = "error", message = $"Can\'t decode solution from BASE64: {e.Message}" });
			}
			if (code.Length > TextsRepo.MaxTextSize)
				return Json(new { status = "error", message = $"Too large solution. Max allowed size is {TextsRepo.MaxTextSize} bytes" });

			var slide = courseManager.FindCourse(courseId)?.FindSlideById(slideId) as ExerciseSlide;
			if (slide == null)
				return Json(new { status = "error", message = "Invalid slide id" });

			var result = await CheckSolution(courseId, slide, code, client.UserId, $"Grader client {client.Name}");

			if (result.IsCompillerFailure)
				return Json(new { status = "error", message = "Can\'t start solution checking, service is temporary unavailable. Try later" });

			var submissionId = result.SubmissionId;
			var solutionByGrader = await gradersRepo.AddSolutionFromGraderClient(clientId, submissionId, "");

			return Json(new { status = "ok", submissionId = solutionByGrader.Id });
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public ActionResult Clients(string courseId)
		{
			var model = new GradesListViewModel
			{
				CourseId = courseId,
				Clients = gradersRepo.GetGraderClients(courseId)
			};
			return View(model);
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		[HttpPost]
		public async Task<ActionResult> AddClient(string courseId, string name)
		{
			await gradersRepo.AddGraderClient(courseId, name);
			return RedirectToAction("Clients", new { courseId = courseId });
		}
	}

	public class GradesListViewModel 
	{
		public string CourseId { get; set; }
		public List<GraderClient> Clients { get; set; }
	}
}