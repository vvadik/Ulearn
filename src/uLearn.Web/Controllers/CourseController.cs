using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using uLearn.Model.Blocks;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly LtiRequestsRepo ltiRequestsRepo = new LtiRequestsRepo();
		private readonly SlideCheckingsRepo slideCheckingsRepo = new SlideCheckingsRepo();
		private readonly GroupsRepo groupsRepo = new GroupsRepo();

		public CourseController()
			: this(WebCourseManager.Instance)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[AllowAnonymous]
		public async Task<ActionResult> SlideById(string courseId, string slideId="", int? checkQueueItemId=null, int? groupId=null)
		{
			if (slideId.Contains("_"))
				slideId = slideId.Substring(slideId.LastIndexOf('_') + 1);

			Guid slideGuid;
			if (!Guid.TryParse(slideId, out slideGuid))
				return HttpNotFound();

			if (string.IsNullOrWhiteSpace(courseId))
			{
				if (string.IsNullOrWhiteSpace(slideId))
					return RedirectToAction("Index", "Home");

				var course = courseManager.FindCourseBySlideById(slideGuid);
				if (course == null)
					return HttpNotFound();
				var slide = course.GetSlideById(slideGuid);
				return RedirectToRoute("Course.SlideById", new { courseId = course.Id, slideId = slide.Url });
			}

			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			var isGuest = !User.Identity.IsAuthenticated;

			AbstractManualSlideChecking queueItem = null;
			if (User.HasAccessFor(courseId, CourseRole.Instructor) && checkQueueItemId != null)
			{
				var course = courseManager.GetCourse(courseId);
				var slide = course.GetSlideById(slideGuid);

				if (slide is QuizSlide)
					queueItem = slideCheckingsRepo.FindManualCheckingById<ManualQuizChecking>(checkQueueItemId.Value);
				if (slide is ExerciseSlide)
					queueItem = slideCheckingsRepo.FindManualCheckingById<ManualExerciseChecking>(checkQueueItemId.Value);

				if (queueItem == null)
					return HttpNotFound();

				/* If lock time is finished or some mistake happened */
				if (!queueItem.IsLockedBy(User.Identity))
					return RedirectToAction(GetAdminQueueActionName(queueItem), "Admin", new { CourseId = courseId, message = "time_is_over" });
			}

			var fillExerciseAgain = Request.QueryString["again"] == "true";

			var model = isGuest ?
				CreateGuestCoursePageModel(courseId, slideGuid, visibleUnits) :
				await CreateCoursePageModel(courseId, slideGuid, visibleUnits, queueItem, fillExerciseAgain, groupId);

			if (!string.IsNullOrEmpty(Request.QueryString["error"]))
				model.Error = Request.QueryString["error"];

			if (!visibleUnits.Contains(model.Slide.Info.UnitName))
				throw new Exception("Slide is hidden " + slideGuid);
			return View("Slide", model);
		}

		private string GetAdminQueueActionName(AbstractManualSlideChecking queueItem)
		{
			if (queueItem is ManualQuizChecking)
				return "ManualQuizCheckingQueue";
			if (queueItem is ManualExerciseChecking)
				return "ManualExerciseCheckingQueue";
			return "";
		}

		[AllowAnonymous]
		public ActionResult Slide(string courseId, int slideIndex = -1)
		{
			var course = courseManager.GetCourse(courseId);
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			if (slideIndex == -1)
				slideIndex = GetInitialIndexForStartup(courseId, course, visibleUnits);
			var slide = course.Slides[slideIndex];
			return RedirectToRoute("Course.SlideById", new { courseId, slideId = slide.Url });
		}

		public async Task<ActionResult> LtiSlide(string courseId, int slideIndex)
		{
			if (string.IsNullOrWhiteSpace(courseId))
				return RedirectToAction("Index", "Home");

			var userId = User.Identity.GetUserId();

			var ltiRequestJson = FindLtiRequestJson();
			var course = courseManager.GetCourse(courseId);
			var slide = course.Slides[slideIndex];

			if (!string.IsNullOrWhiteSpace(ltiRequestJson))
				await ltiRequestsRepo.Update(userId, slide.Id, ltiRequestJson);

			var visiter = await VisitSlide(courseId, slide.Id, userId);

			var exerciseSlide = slide as ExerciseSlide;
			if (exerciseSlide != null)
			{
				var model = new CodeModel
				{
					CourseId = courseId,
					SlideIndex = slideIndex,
					SlideId = exerciseSlide.Id,
					ExerciseBlock = exerciseSlide.Exercise,
					Context = CreateRenderContext(course, exerciseSlide, visiter, true)
				};
				return View("LtiExerciseSlide", model);
			}

			var quizSlide = slide as QuizSlide;
			if (quizSlide != null)
			{
				var model = new LtiQuizModel
				{
					CourseId = courseId,
					Slide = quizSlide,
					UserId = userId
				};
				return View("LtiQuizSlide", model);
			}

			return View();
		}

		private string FindLtiRequestJson()
		{
			var user = User.Identity as ClaimsIdentity;
			var claim = user?.Claims.FirstOrDefault(c => c.Type.Equals("LtiRequest"));
			return claim?.Value;
		}

		private int GetInitialIndexForStartup(string courseId, Course course, List<string> visibleUnits)
		{
			var userId = User.Identity.GetUserId();
			var visitedIds = visitsRepo.GetIdOfVisitedSlides(courseId, userId);
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
		
		private CoursePageModel CreateGuestCoursePageModel(string courseId, Guid slideId, List<string> visibleUnits)
		{
			var course = courseManager.GetCourse(courseId);
			Slide slide;
			if (slideId == Guid.Empty)
			{
				var slideIndex = GetInitialIndexForStartup(courseId, course, visibleUnits);
				slide = course.Slides[slideIndex];
			}
			else
				slide = course.GetSlideById(slideId);
			var exerciseBlockData = new ExerciseBlockData(courseId, slide.Index, false, false) { Url = Url };

			return new CoursePageModel
			{
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = slide,
				Score = Tuple.Create(0, 0),
				BlockRenderContext = new BlockRenderContext(
					course,
					slide,
					slide.Info.DirectoryRelativePath,
					slide.Blocks.Select(block => block is ExerciseBlock ? exerciseBlockData : (dynamic)null).ToArray(),
					true),
				IsGuest = true,
			};
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, Guid slideId, List<string> visibleUnits, AbstractManualSlideChecking manualChecking, bool fillExerciseAgain=false, int? groupId=null)
		{
			var course = courseManager.GetCourse(courseId);

			Slide slide;
			if (slideId == Guid.Empty)
			{
				var slideIndex = GetInitialIndexForStartup(courseId, course, visibleUnits);
				slide = course.Slides[slideIndex];
			}
			else
				slide = course.GetSlideById(slideId);

			var userId = User.Identity.GetUserId();

			if (manualChecking != null)
				userId = manualChecking.UserId;

			var visiter = await VisitSlide(courseId, slideId, userId);
			var score = Tuple.Create(visiter.Score, slide.MaxScore);
			var model = new CoursePageModel
			{
				UserId = userId,
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = slide,
				Rate = GetRate(course.Id, slideId),
				Score = score,
				BlockRenderContext = CreateRenderContext(course, slide, visiter, false, manualChecking, fillExerciseAgain, groupId),
				ManualChecking = manualChecking,
				ContextManualCheckingUserGroups = manualChecking != null ? groupsRepo.GetUserGroupsNamesAsString(course.Id, manualChecking.UserId, User) : "",
				IsGuest = false,
			};
			return model;
		}

		private BlockRenderContext CreateRenderContext(Course course, Slide slide, Visit visit, bool isLti = false, AbstractManualSlideChecking manualChecking=null, bool fillExerciseAgain=false, int? groupId=null)
		{
			var blockData = slide.Blocks.Select(b => CreateBlockData(course, slide, b, visit, isLti, manualChecking, fillExerciseAgain)).ToArray();
			return new BlockRenderContext(
				course,
				slide,
				slide.Info.DirectoryRelativePath,
				blockData,
				false,
				User.HasAccessFor(course.Id, CourseRole.Instructor),
				manualChecking,
				false,
				groupId
				);
		}

		private dynamic CreateBlockData(Course course, Slide slide, SlideBlock slideBlock, Visit visit, bool isLti, AbstractManualSlideChecking manualChecking, bool fillExerciseAgain = false)
		{
			if (slideBlock is ExerciseBlock)
			{
				var submission = manualChecking != null ?
					((ManualExerciseChecking)manualChecking).Submission :
					solutionsRepo.FindLatestAcceptedSubmission(course.Id, slide.Id, visit.UserId);
				var solution = submission?.SolutionCode.Text;
				var submissionReviews = submission?.ManualCheckings.LastOrDefault()?.Reviews.Where(r => ! r.IsDeleted);

				var hasUncheckedReview = submission?.ManualCheckings.Any(c => !c.IsChecked) ?? false;
				var hasCheckedReview = submission?.ManualCheckings.Any(c => c.IsChecked) ?? false;
				var reviewState = hasCheckedReview ? ExerciseReviewState.Reviewed :
					hasUncheckedReview ? ExerciseReviewState.WaitingForReview :
						ExerciseReviewState.NotReviewed;
				if (fillExerciseAgain)
					reviewState = ExerciseReviewState.NotReviewed;

				return new ExerciseBlockData(course.Id, slide.Index, true, visit.IsSkipped, solution)
				{
					Url = Url,
					Reviews = submissionReviews?.ToList() ?? new List<ExerciseCodeReview>(),
					IsLti = isLti,
					ReviewState = reviewState,
				};
			}
			return null;
		}

		public async Task<ViewResult> AcceptedSolutions(string courseId, int slideIndex = 0, bool isLti = false)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isPassed = visitsRepo.IsPassed(slide.Id, userId);
			if (!isPassed)
				await visitsRepo.SkipSlide(courseId, slide.Id, userId);
			var solutions = solutionsRepo.GetBestTrendingAndNewAcceptedSolutions(courseId, slide.Id);
			foreach (var solution in solutions)
			{
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);
				solution.RemoveSolutionUrl = Url.Action("RemoveSolution", "Course", new { courseId, slideIndex = slide.Index, solutionId = solution.Id });
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

		public ViewResult AcceptedAlert(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isSkippedOrPassed = visitsRepo.IsSkippedOrPassed(slide.Id, userId);
			var model = new ExerciseBlockData(courseId, slide.Index)
			{
				IsSkippedOrPassed = isSkippedOrPassed,
				CourseId = courseId,
				Url = Url,
				SlideIndex = slideIndex
			};
			return View(model);
		}

		[HttpPost]
		public async Task<string> ApplyRate(string courseId, Guid slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates)Enum.Parse(typeof(SlideRates), rate);
			return await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
		}

		[HttpPost]
		public string GetRate(string courseId, Guid slideId)
		{
			var userId = User.Identity.GetUserId();
			return slideRateRepo.FindRate(courseId, slideId, userId);
		}

		[HttpPost]
		public async Task<JsonResult> LikeSolution(int solutionId)
		{
			var res = await solutionsRepo.Like(solutionId, User.Identity.GetUserId());
			return Json(new { likesCount = res.Item1, liked = res.Item2 });
		}

		public async Task<Visit> VisitSlide(string courseId, Guid slideId, string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return null;
			await visitsRepo.AddVisit(courseId, slideId, userId);
			return visitsRepo.GetVisiter(slideId, userId);
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public ActionResult InstructorNote(string courseId, string unitName)
		{
			InstructorNote instructorNote = courseManager.GetCourse(courseId).FindInstructorNote(unitName);
			if (instructorNote == null)
				return HttpNotFound("no instructor note for this unit");
			return View(instructorNote);
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
		public async Task<ActionResult> RemoveSolution(string courseId, int slideIndex, int solutionId)
		{
			var submission = await db.UserExerciseSubmissions.FirstOrDefaultAsync(s => s.Id == solutionId);
			if (submission != null)
			{
				var visit = await db.Visits.FirstOrDefaultAsync(v => v.UserId == submission.UserId && v.SlideId == submission.SlideId);
				if (visit != null)
				{
					/* TODO(andgein): Replace to UpdateScoreForVisit() */
					visit.IsPassed = db.UserExerciseSubmissions.Any(s => s.UserId == submission.UserId && s.SlideId == submission.SlideId && s.AutomaticChecking.IsRightAnswer && s.Id != solutionId);
					visit.Score = visit.IsSkipped ? 0 : visit.IsPassed ? 5 : 0; //TODO fix 5 to Score from UserSolution
					visit.AttemptsCount--;
				}
				db.UserExerciseSubmissions.Remove(submission);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("AcceptedSolutions", new { courseId, slideIndex });
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.Tester)]
		public async Task<ActionResult> ForgetAll(string courseId, Guid slideId)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			var userId = User.Identity.GetUserId();
			db.SolutionLikes.RemoveRange(db.SolutionLikes.Where(q => q.UserId == userId && q.Submission.SlideId == slideId));

			RemoveFrom(db.UserExerciseSubmissions, slideId, userId);
			RemoveFrom(db.UserQuizzes, slideId, userId);
			RemoveFrom(db.Visits, slideId, userId);
			await slideCheckingsRepo.RemoveAttempts(courseId, slideId, userId, false);
			db.UserQuestions.RemoveRange(db.UserQuestions.Where(q => q.UserId == userId && q.SlideId == slideId));
			db.SlideRates.RemoveRange(db.SlideRates.Where(q => q.UserId == userId && q.SlideId == slideId));
			db.Hints.RemoveRange(db.Hints.Where(q => q.UserId == userId && q.SlideId == slideId));
			await db.SaveChangesAsync();

			return RedirectToAction("Slide", new { courseId, slideIndex = slide.Index });
		}

		private static void RemoveFrom<T>(DbSet<T> dbSet, Guid slideId, string userId) where T : class, ISlideAction
		{
			dbSet.RemoveRange(dbSet.Where(s => s.UserId == userId && s.SlideId == slideId));
		}

		public ActionResult CourseInstructorNavbar(string courseId)
		{
			if (string.IsNullOrEmpty(courseId))
				return PartialView((CourseInstructorNavbarViewModel)null);

			var course = courseManager.GetCourse(courseId);
			return PartialView(new CourseInstructorNavbarViewModel
			{
				CourseId = courseId,
				CourseTitle = course.Title,
			});
		}
	}
}
 