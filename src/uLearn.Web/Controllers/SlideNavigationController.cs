using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class SlideNavigationController : Controller
	{
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		
		public ActionResult TableOfContents(string courseId, int slideIndex = -1)
		{
			var course = courseManager.GetCourse(courseId);
			var userId = User.Identity.GetUserId();
			var solvedSlides = solutionsRepo.GetIdOfPassedSlides(course.Id, userId);
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			solvedSlides.UnionWith(userQuizzesRepo.GetIdOfQuizPassedSlides(course.Id, userId));
			var units = course.Slides
				.Where(s => visibleUnits.Contains(s.Info.UnitName))
				.GroupBy(
					s => s.Info.UnitName,
					(unitName, slides) => new CourseUnitModel { Slides = slides.ToArray(), InstructorNote = course.GetInstructorNote(unitName), UnitName = unitName })
				.ToArray();
			var visitedSlideIds = visitersRepo.GetIdOfVisitedSlides(course.Id, userId);
			var currentSlide = course.FindSlide(slideIndex);
			if (currentSlide != null && !visibleUnits.Contains(currentSlide.Info.UnitName))
				currentSlide = null;
			var currentUnit = units.FirstOrDefault(u => u.Slides.Contains(currentSlide) && visibleUnits.Contains(u.UnitName));
			return PartialView(new TocModel
			{
				CourseId = course.Id,
				CurrentSlide = currentSlide,
				CurrentUnit = currentUnit,
				Units = units,
				VisitedSlideIds = visitedSlideIds,
				SolvedSlideIds = solvedSlides,
			});
		}

		public ActionResult PrevNextButtons(string courseId, int slideIndex, bool onSolutionsSlide)
		{
			var course = courseManager.GetCourse(courseId);
			var slide = course.Slides[slideIndex];
			var userId = User.Identity.GetUserId();
			var isPassedTask = solutionsRepo.IsUserPassedTask(courseId, slide.Id, userId);
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			var nextSlide = course.Slides.FirstOrDefault(s => s.Index > slideIndex && visibleUnits.Contains(s.Info.UnitName));
			var prevSlide = course.Slides.LastOrDefault(s => s.Index < slideIndex && visibleUnits.Contains(s.Info.UnitName));
			var model = new PrevNextButtonsModel(course, slideIndex, isPassedTask, onSolutionsSlide, nextSlide == null ? -1 : nextSlide.Index, prevSlide == null ? -1 : prevSlide.Index);
			if (onSolutionsSlide) model.PrevSlideIndex = model.SlideIndex;
			return PartialView(model);

		}

	}
}