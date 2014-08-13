using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class HintController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UserQuestionsRepo userQuestionsRepo = new UserQuestionsRepo();
		private readonly ExecutionService executionService = new ExecutionService();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

		public HintController()
			: this(CourseManager.AllCourses)
		{
		}

		public HintController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[HttpPost]
		[Authorize]
		public async Task<string> UseHint(string courseId, string slideId, int hintId)
		{
			var userId = User.Identity.GetUserId();
			await slideHintRepo.AddHint(userId, hintId, courseId, slideId);
			return "success";
		}

		[HttpPost]
		[Authorize]
		public ActionResult GetHint(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			var answer = slideHintRepo.GetUsedHintId(userId, courseId, slideId);
			return Json(answer);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> LikeHint(string courseId, string slideId, int hintId)
		{
			return await slideHintRepo.LikeHint(courseId, slideId, hintId, User.Identity.GetUserId());
		}
	}
}