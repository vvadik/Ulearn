using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Model.Blocks;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CodeController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly LtiRequestsRepo ltiRequestsRepo = new LtiRequestsRepo();

		public CodeController()
			: this(WebCourseManager.Instance)
		{
		}

		public CodeController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public async Task<ActionResult> Slide(string courseId, int slideIndex)
		{
			if (string.IsNullOrWhiteSpace(courseId))
				return RedirectToAction("Index", "Home");

			var userId = User.Identity.GetUserId();

			var ltiRequestJson = FindLtiRequestJson();
			var course = courseManager.GetCourse(courseId);
			var slide = course.Slides[slideIndex] as ExerciseSlide;
			if (!string.IsNullOrWhiteSpace(ltiRequestJson))
				await ltiRequestsRepo.Update(userId, slide.Id, ltiRequestJson);

			await visitersRepo.AddVisiter(courseId, slide.Id, userId);
			var visiter = visitersRepo.GetVisiter(slide.Id, User.Identity.GetUserId());
			var model = new CodeModel
			{
				CourseId = courseId,
				SlideIndex = slideIndex,
				ExerciseBlock = slide.Exercise,
				Context = CreateRenderContext(course, slide, userId, visiter)
			};
			return View(model);
		}

		private BlockRenderContext CreateRenderContext(Course course, Slide slide, string userId, Visiters visiter)
		{
			var blockData = slide.Blocks.Select(b => CreateBlockData(course, slide, b, visiter)).ToArray();
			return new BlockRenderContext(
				course,
				slide,
				slide.Info.DirectoryRelativePath,
				blockData);
		}

		private dynamic CreateBlockData(Course course, Slide slide, SlideBlock slideBlock, Visiters visiter)
		{
			if (slideBlock is ExerciseBlock)
			{
				var lastAcceptedSolution = solutionsRepo.FindLatestAcceptedSolution(course.Id, slide.Id, visiter.UserId);
				return new ExerciseBlockData(true, visiter.IsSkipped, lastAcceptedSolution)
				{
					RunSolutionUrl = Url.Action("RunSolution", "Exercise", new { courseId = course.Id, slideIndex = slide.Index, isLti = true }),
					AcceptedSolutionUrl = Url.Action("AcceptedSolutions", "Course", new { courseId = course.Id, slideIndex = slide.Index }),
					GetHintUrl = Url.Action("UseHint", "Hint"),
					IsLti = true
				};
			}
			return null;
		}

		private string FindLtiRequestJson()
		{
			var user = User.Identity as ClaimsIdentity;
			if (user == null)
				return null;

			var claim = user.Claims.FirstOrDefault(c => c.Type.Equals("LtiRequest"));
			return claim == null ? null : claim.Value;
		}
	}
}