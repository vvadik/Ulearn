using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
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
			var nextUnitTime = unitsRepo.GetNextUnitPublishTime(courseId);

			var scoresForSlides = GetScoresForSlides(course, userId);
			var scoresForUnits = GetScoresForUnits(scoresForSlides);
			var scoreForCourse = GetScoreForCourse(scoresForUnits);

			return PartialView(new TocModel
			{
				CourseId = course.Id,
				CourseName = course.Title,
				CurrentSlide = currentSlide,
				CurrentUnit = currentUnit,
				Units = units,
				VisitedSlideIds = visitedSlideIds,
				SolvedSlideIds = solvedSlides,
				NextUnitTime = nextUnitTime,
				ScoresForSlides = scoresForSlides,
				ScoresForUnits = scoresForUnits,
				ScoreForCourse = scoreForCourse
			});
		}

		public ActionResult PrevNextButtons(string courseId, int slideIndex, bool onSolutionsSlide)
		{
			var course = courseManager.GetCourse(courseId);
			var slide = course.Slides[slideIndex];
			var userId = User.Identity.GetUserId();
			var nextIsAcceptedSolutions = !onSolutionsSlide && slide is ExerciseSlide && visitersRepo.IsSkippedOrPassed(slide.Id, userId);
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			var nextSlide = course.Slides.FirstOrDefault(s => s.Index > slideIndex && visibleUnits.Contains(s.Info.UnitName));
			var prevSlide = course.Slides.LastOrDefault(s => s.Index < slideIndex && visibleUnits.Contains(s.Info.UnitName));
			
			var model = new PrevNextButtonsModel(course, slideIndex, nextIsAcceptedSolutions, nextSlide == null ? -1 : nextSlide.Index, prevSlide == null ? -1 : prevSlide.Index);
			if (onSolutionsSlide) model.PrevSlideIndex = model.SlideIndex;
			return PartialView(model);
		}

		private Dictionary<Slide, Tuple<int, int>> GetScoresForSlides(Course course, string userId)
		{
			var scoresForSlides = visitersRepo.GetScoresForSlides(course.Id, userId);
			return course.Slides
				.ToDictionary(slide => slide, slide => Tuple.Create(scoresForSlides.Get(slide.Id, 0), slide.MaxScore));
		}

		private Dictionary<string, Tuple<int, int>> GetScoresForUnits(Dictionary<Slide, Tuple<int, int>> scoresForSlides)
		{
			return scoresForSlides
				.GroupBy(pair => pair.Key.Info.UnitName)
				.ToDictionary(
					pairs => pairs.Key, 
					pairs => Sum(pairs.Select(pair => pair.Value)));
		}

		private Tuple<int, int> GetScoreForCourse(Dictionary<string, Tuple<int, int>> scoresForUnits)
		{
			return Sum(scoresForUnits.Values);
		}

		private Tuple<int, int> Sum(IEnumerable<Tuple<int, int>> tuples)
		{
			return tuples.Aggregate(Tuple.Create(0, 0), (t1, t2) => Tuple.Create(t1.Item1 + t2.Item1, t1.Item2 + t2.Item2));
		}
	}
}