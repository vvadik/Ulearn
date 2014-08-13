using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class HintController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();

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
		public async Task<ActionResult> UseHint(string courseId, int slideIndex, bool isNeedNewHint)
		{
			var slide = courseManager.GetCourse(courseId).Slides[slideIndex];
			if (!(slide is ExerciseSlide))
				return Json(new { Text = "Для слайда нет подсказок" });
			var exerciseSlide = (ExerciseSlide) slide;
			if (exerciseSlide.HintsHtml.Length == 0)
				return Json(new { Text = "Для слайда нет подсказок" });
			var a = new HintPageModel {Hints = await GetNewHintHtml(exerciseSlide, courseId, isNeedNewHint)};
			if (a.Hints == null)
				return Json(new {Text = "Подсказок больше нет"});
			return PartialView("~/Views/Course/_ExerciseHint.cshtml", a);
		}

		private async Task<HintWithLikeButton[]> GetNewHintHtml(ExerciseSlide exerciseSlide, string courseId, bool isNeedNewHint)
		{
			var usedHintsCount = slideHintRepo.GetUsedHintsCount(courseId, exerciseSlide.Id, User.Identity.GetUserId());
			if (usedHintsCount != exerciseSlide.HintsHtml.Length)
				return await RenderHtmlWithHint(exerciseSlide, isNeedNewHint ? usedHintsCount : usedHintsCount - 1, courseId);
			if (isNeedNewHint)
				return null;
			return await RenderHtmlWithHint(exerciseSlide, usedHintsCount - 1, courseId);
		}

		private async Task<HintWithLikeButton[]> RenderHtmlWithHint(ExerciseSlide exerciseSlide, int hintsCount, string courseId)
		{
			var ans = new HintWithLikeButton[hintsCount + 1];
			for (var i = 0; i <= hintsCount; i++)
			{
				var isLiked = slideHintRepo.IsHintLiked(courseId, exerciseSlide.Id, User.Identity.GetUserId(), i);
				ans[i] = await MakeExerciseHint(exerciseSlide.HintsHtml[i], i, courseId, exerciseSlide.Id, isLiked);
			}
			return ans;
		}

		private async Task<HintWithLikeButton> MakeExerciseHint(string hintText, int hintId, string courseId, string slideId, bool isLiked)
		{
			await slideHintRepo.AddHint(User.Identity.GetUserId(), hintId, courseId, slideId);
			return new HintWithLikeButton
			{
				CourseId = courseId,
				Hint = hintText,
				HintId = hintId,
				IsLiked = isLiked,
				SlideId = slideId
			};
		}

		[HttpPost]
		[Authorize]
		public async Task<string> LikeHint(string courseId, string slideId, int hintId)
		{
			return await slideHintRepo.LikeHint(courseId, slideId, hintId, User.Identity.GetUserId());
		}
	}
}