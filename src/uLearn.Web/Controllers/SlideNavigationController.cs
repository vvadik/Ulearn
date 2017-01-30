using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class SlideNavigationController : Controller
	{
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		private readonly GroupsRepo groupsRepo = new GroupsRepo();
		
		public ActionResult TableOfContents(string courseId, Guid? slideId = null)
		{
			var course = courseManager.GetCourse(courseId);
			var userId = User.Identity.GetUserId();
			var model = User.Identity.IsAuthenticated ? CreateTocModel(course, slideId, userId) : CreateGuestTocModel(course, slideId);
			return PartialView(model);
		}

		private TocModel CreateGuestTocModel(Course course, Guid? currentSlideId)
		{
			var visibleUnits = unitsRepo.GetVisibleUnits(course, User);
			var builder = new TocModelBuilder(
				s => Url.RouteUrl("Course.SlideById", new { courseId = course.Id, slideId = s.Url }),
				s => 0,
				s => 0,
				course,
				currentSlideId);
			builder.IsInstructor = false;
			builder.IsVisited = s => false;
			builder.IsUnitVisible = visibleUnits.Contains;
			builder.IsSlideHidden = s => s is QuizSlide && ((QuizSlide)s).Quiz.ManualCheck;
			var toc = builder.CreateTocModel();
			toc.NextUnitTime = unitsRepo.GetNextUnitPublishTime(course.Id);
			return toc;
		}

		private HashSet<Guid> GetSolvedSlides(Course course, string userId)
		{
			var solvedSlides = solutionsRepo.GetIdOfPassedSlides(course.Id, userId);
			solvedSlides.UnionWith(userQuizzesRepo.GetIdOfQuizPassedSlides(course.Id, userId));
			return solvedSlides;
		}

		private int GetMaxScoreForUsersSlide(Slide slide, bool isSolved, bool hasManualChecking, bool enabledManualCheckingForUser)
		{
			var isExerciseOrQuiz = slide is ExerciseSlide || slide is QuizSlide;

			if (!isExerciseOrQuiz)
				return slide.MaxScore;

			if (isSolved)
				return hasManualChecking ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
			else
				return enabledManualCheckingForUser ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
		}

		private int GetMaxScoreWithoutManualChecking(Slide slide)
		{
			if (slide is ExerciseSlide)
				return (slide as ExerciseSlide).Exercise.CorrectnessScore;
			if (slide is QuizSlide)
				return (slide as QuizSlide).Quiz.ManualCheck ? 0 : slide.MaxScore;
			return slide.MaxScore;
		}

		private TocModel CreateTocModel(Course course, Guid? currentSlideId, string userId)
		{
			var visibleUnits = unitsRepo.GetVisibleUnits(course, User);
			var solvedSlidesIds = GetSolvedSlides(course, userId);
			var visited = visitsRepo.GetIdOfVisitedSlides(course.Id, userId);
			var scoresForSlides = visitsRepo.GetScoresForSlides(course.Id, userId);
			var slidesWithUsersManualChecking = visitsRepo.GetSlidesWithUsersManualChecking(course.Id, userId).ToImmutableHashSet();
			var enabledManualCheckingForUser = groupsRepo.IsManualCheckingEnabledForUser(course, userId);

			var builder = new TocModelBuilder(
				s => Url.RouteUrl("Course.SlideById", new { courseId = course.Id, slideId = s.Url }),
				s => scoresForSlides.ContainsKey(s.Id) ? scoresForSlides[s.Id] : 0,
				s => GetMaxScoreForUsersSlide(s, solvedSlidesIds.Contains(s.Id), slidesWithUsersManualChecking.Contains(s.Id), enabledManualCheckingForUser),
				course,
				currentSlideId)
			{
				GetUnitInstructionNotesUrl = unit => Url.Action("InstructorNote", "Course", new { courseId = course.Id, unitId = unit.Id }),
				GetUnitStatisticsUrl = unit => Url.Action("UnitStatistics", "Analytics", new { courseId = course.Id, unitId = unit.Id }),
				IsInstructor = User.HasAccessFor(course.Id, CourseRole.Instructor),
				IsSolved = s => solvedSlidesIds.Contains(s.Id),
				IsVisited = s => visited.Contains(s.Id),
				IsUnitVisible = visibleUnits.Contains,
				IsSlideHidden = s => s is QuizSlide && ((QuizSlide)s).Quiz.ManualCheck &&
									!enabledManualCheckingForUser && !solvedSlidesIds.Contains(s.Id)
			};

			var toc = builder.CreateTocModel();
			toc.NextUnitTime = unitsRepo.GetNextUnitPublishTime(course.Id);
			return toc;
		}

		public ActionResult PrevNextButtons(string courseId, Guid slideId, bool onSolutionsSlide)
		{
			var course = courseManager.GetCourse(courseId);
			var slide = course.GetSlideById(slideId);
			var userId = User.Identity.GetUserId();
			var nextIsAcceptedSolutions = !onSolutionsSlide && slide is ExerciseSlide && visitsRepo.IsSkippedOrPassed(slide.Id, userId) && !((ExerciseSlide)slide).Exercise.HideShowSolutionsButton;
			var visibleUnits = unitsRepo.GetVisibleUnits(course, User);
			var nextSlide = course.Slides.FirstOrDefault(s => s.Index > slide.Index && visibleUnits.Contains(s.Info.Unit));
			var prevSlide = course.Slides.LastOrDefault(s => s.Index < slide.Index && visibleUnits.Contains(s.Info.Unit));
			
			var model = new PrevNextButtonsModel(
				course, 
				slide.Id, 
				nextIsAcceptedSolutions, 
				nextSlide,
				prevSlide, 
				!User.Identity.IsAuthenticated);
			if (onSolutionsSlide) model.PrevSlide = slide;
			return PartialView(model);
		}
	}
}