using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();

		public CourseController()
			: this(WebCourseManager.Instance)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public async Task<ActionResult> Slide(string courseId, int slideIndex = -1)
		{
			if (string.IsNullOrWhiteSpace(courseId)) return RedirectToAction("Index", "Home");
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			var model = await CreateCoursePageModel(courseId, slideIndex, visibleUnits);
			if (!visibleUnits.Contains(model.Slide.Info.UnitName))
				throw new Exception("Slide is hidden " + slideIndex);
			var quizSlide = model.Slide as QuizSlide;
			if (quizSlide != null)
				foreach (var block in quizSlide.Quiz.Blocks.Where(x => x is ChoiceBlock).Where(x => ((ChoiceBlock) x).Shuffle))
					Shuffle(block);
			return View(model);
		}

		private void Shuffle(QuizBlock quizBlock)
		{
			var random = new Random();
			var choiceBlock = (ChoiceBlock) quizBlock;
			choiceBlock.Items = choiceBlock.Items.OrderBy(x => random.Next()).ToArray();
		}

		private int GetInitialIndexForStartup(string courseId, Course course, List<string> visibleUnits)
		{
			var userId = User.Identity.GetUserId();
			var lastVisitedSlide = 0;
			for (var index = 0; index < course.Slides.Length; index++)
			{
				var slide = course.Slides[index];
				if (!visibleUnits.Contains(slide.Info.UnitName)) continue;
				if (visitersRepo.IsUserVisit(courseId, slide.Id, userId))
					lastVisitedSlide = index;
				else
					return lastVisitedSlide;
			}
			return lastVisitedSlide;
		}

		[Authorize]
		[HttpGet]
		public ActionResult SelectGroup()
		{
			var groups = db.Users.Select(u => u.GroupName).Distinct().OrderBy(g => g).ToArray();
			return PartialView(groups);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> SelectGroup(string groupName)
		{
			var userId = User.Identity.GetUserId();
			var user = db.Users.FirstOrDefault(x => x.Id == userId);
			if (user != null)
			{
				user.GroupName = groupName;
				await db.SaveChangesAsync();
				return Content("");
			}
			return HttpNotFound("User not found");
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, int slideIndex, List<string> visibleUnits)
		{
			var course = courseManager.GetCourse(courseId);
			if (slideIndex == -1)
				slideIndex = GetInitialIndexForStartup(courseId, course, visibleUnits);
			var userId = User.Identity.GetUserId();
			var isFirstCourseVisit = !db.Visiters.Any(x => x.UserId == userId);
			var slideId = course.Slides[slideIndex].Id;
			await VisitSlide(courseId, slideId);
			var model = new CoursePageModel
			{
				UserId = userId,
				IsFirstCourseVisit = isFirstCourseVisit,
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = course.Slides[slideIndex],
				LatestAcceptedSolution =
					solutionsRepo.FindLatestAcceptedSolution(courseId, slideId, userId),
				Rate = GetRate(course.Id, slideId),
			};
			return model;
		}

		[Authorize]
		public ActionResult AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isPassed = solutionsRepo.IsUserPassedTask(courseId, slide.Id, userId);
			var solutions = isPassed
				? solutionsRepo.GetAllAcceptedSolutions(courseId, slide.Id)
				: new List<AcceptedSolutionInfo>();
			foreach (var solution in solutions)
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);
			var model = new AcceptedSolutionsPageModel
			{
				CourseId = courseId,
				CourseTitle = course.Title,
				Slide = slide,
				AcceptedSolutions = solutions
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyRate(string courseId, string slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates) Enum.Parse(typeof (SlideRates), rate);
			return await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
		}

		[HttpPost]
		[Authorize]
		public string GetRate(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			return slideRateRepo.FindRate(courseId, slideId, userId);
		}

		[HttpPost]
		[Authorize]
		public async Task<JsonResult> LikeSolution(int solutionId)
		{
			var res = await solutionsRepo.Like(solutionId, User.Identity.GetUserId());
			return Json(new {likesCount = res.Item1, liked = res.Item2});
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
		}

		[Authorize(Roles = LmsRoles.Instructor)]
		public ActionResult InstructorNote(string courseId, string unitName)
		{
			InstructorNote instructorNote = courseManager.GetCourse(courseId).GetInstructorNote(unitName);
			return View(instructorNote);
		}

		[HttpPost]
		[Authorize(Roles=LmsRoles.Instructor + "," + LmsRoles.Admin)]
		public async Task<ActionResult> RemoveSolution(string courseId, int slideIndex, int solutionId)
		{
			var solution = await db.UserSolutions.FirstOrDefaultAsync(s => s.Id == solutionId);
			if (solution != null)
			{
				db.UserSolutions.Remove(solution);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("AcceptedSolutions", new { courseId = courseId, slideIndex = slideIndex });
		}
	}
}