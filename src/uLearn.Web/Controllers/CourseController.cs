using System;
using System.Collections.Generic;
using System.Data.Entity;
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
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo(); 
		private readonly LtiRequestsRepo ltiRequestsRepo = new LtiRequestsRepo();

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
			return View(model);
		}

		[Authorize]
		public async Task<ActionResult> LtiSlide(string courseId, int slideIndex)
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

		private string FindLtiRequestJson()
		{
			var user = User.Identity as ClaimsIdentity;
			if (user == null)
				return null;

			var claim = user.Claims.FirstOrDefault(c => c.Type.Equals("LtiRequest"));
			return claim == null ? null : claim.Value;
		}

		private int GetInitialIndexForStartup(string courseId, Course course, List<string> visibleUnits)
		{
			var userId = User.Identity.GetUserId();
			var visitedIds = visitersRepo.GetIdOfVisitedSlides(courseId, userId);
			var visibleSlides = course.Slides.Where(slide => visibleUnits.Contains(slide.Info.UnitName)).OrderBy(slide => slide.Index).ToList();
			var lastVisited = visibleSlides.LastOrDefault(slide => visitedIds.Contains(slide.Id));
			if (lastVisited == null)
				return visibleSlides.Any() ? visibleSlides.First().Index : 0;

			var slides = visibleSlides.Where(slide => lastVisited.Info.UnitName == slide.Info.UnitName).ToList();

			var lastVisitedSlide = slides.First().Index;
			foreach (var slide in slides)
			{
				if (visitedIds.Contains(slide.Id))
					lastVisitedSlide = slide.Index;
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
			var slide = course.Slides[slideIndex];
			var slideId = slide.Id;
			await VisitSlide(courseId, slideId);
			var visiter = visitersRepo.GetVisiter(slideId, User.Identity.GetUserId());
			var score = Tuple.Create(visiter.Score, slide.MaxScore);
			var model = new CoursePageModel
			{
				UserId = userId,
				IsFirstCourseVisit = isFirstCourseVisit,
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = slide,
				Rate = GetRate(course.Id, slideId),
				Score = score,
				BlockRenderContext = CreateRenderContext(course, slide, userId, visiter)
			};
			return model;
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
					RunSolutionUrl = Url.Action("RunSolution", "Exercise", new { courseId = course.Id, slideIndex = slide.Index }),
					AcceptedSolutionUrl = Url.Action("AcceptedSolutions", "Course", new { courseId = course.Id, slideIndex = slide.Index }),
					GetHintUrl = Url.Action("UseHint", "Hint")
				};
			}
			return null;
		}

		[Authorize]
		public async Task<ViewResult> AcceptedSolutions(string courseId, int slideIndex = 0, bool isLti = false)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isPassed = visitersRepo.IsPassed(slide.Id, userId);
			if (!isPassed)
				await visitersRepo.SkipSlide(courseId, slide.Id, userId);
			var solutions = solutionsRepo.GetAllAcceptedSolutions(courseId, slide.Id);
			foreach (var solution in solutions)
			{
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);
				solution.RemoveSolutionUrl = Url.Action("RemoveSolution", "Course", new { courseId = courseId, slideIndex = slide.Index, solutionId = solution.Id });
			}
			
			var model = new AcceptedSolutionsPageModel
			{
				CourseId = courseId,
				CourseTitle = course.Title,
				Slide = slide,
				AcceptedSolutions = solutions,
				User = User,
				LikeSolutionUrl = Url.Action("LikeSolution"),
				IsLti = isLti,
				IsPassed = isPassed
			};
			return View(model);
		}

		[Authorize]
		public ViewResult AcceptedAlert(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isSkippedOrPassed = visitersRepo.IsSkippedOrPassed(slide.Id, userId);
			var model = new ExerciseBlockData
			{
				IsSkippedOrPassed = isSkippedOrPassed,
				AcceptedSolutionUrl = Url.Action("AcceptedSolutions", "Course", new { courseId, slideIndex, isLti = true }),
				CourseId = courseId,
				SlideIndex = slideIndex
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyRate(string courseId, string slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates)Enum.Parse(typeof(SlideRates), rate);
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
			return Json(new { likesCount = res.Item1, liked = res.Item2 });
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
		[Authorize(Roles = LmsRoles.Instructor + "," + LmsRoles.Admin)]
		public async Task<ActionResult> RemoveSolution(string courseId, int slideIndex, int solutionId)
		{
			var solution = await db.UserSolutions.FirstOrDefaultAsync(s => s.Id == solutionId);
			if (solution != null)
			{
				var visit = await db.Visiters.FirstOrDefaultAsync(v => v.UserId == solution.UserId && v.SlideId == solution.SlideId);
				if (visit != null)
				{
					visit.IsPassed = db.UserSolutions.Any(s => s.UserId == solution.UserId && s.SlideId == solution.SlideId && s.IsRightAnswer && s.Id != solutionId);
					visit.Score = visit.IsSkipped ? 0 : visit.IsPassed ? 5 : 0;
					visit.AttemptsCount--;
				}
				db.UserSolutions.Remove(solution);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("AcceptedSolutions", new { courseId = courseId, slideIndex = slideIndex });
		}

		[Authorize(Roles = LmsRoles.Tester)]
		public async Task<ActionResult> ForgetAll(string courseId, string slideId)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			var userId = User.Identity.GetUserId();
			db.SolutionLikes.RemoveRange(db.SolutionLikes.Where(q => q.UserId == userId && q.UserSolution.SlideId == slideId));
			RemoveFrom(db.UserSolutions, slideId, userId);
			RemoveFrom(db.UserQuizzes, slideId, userId);
			RemoveFrom(db.Visiters, slideId, userId);
			db.UserQuestions.RemoveRange(db.UserQuestions.Where(q => q.UserId == userId && q.SlideId == slideId));
			db.SlideRates.RemoveRange(db.SlideRates.Where(q => q.UserId == userId && q.SlideId == slideId));
			db.Hints.RemoveRange(db.Hints.Where(q => q.UserId == userId && q.SlideId == slideId));
			await db.SaveChangesAsync();
			return RedirectToAction("Slide", new { courseId, slideIndex = slide.Index });
		}

		private static void RemoveFrom<T>(DbSet<T> dbSet, string slideId, string userId) where T : class, ISlideAction
		{
			dbSet.RemoveRange(dbSet.Where(s => s.UserId == userId && s.SlideId == slideId));
		}
	}
}