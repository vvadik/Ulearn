using System.Linq;
using Database;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using uLearn.Quizes;
using Web.Api.Models.Common;
using SlideInfo = Web.Api.Models.Common.SlideInfo;

namespace Web.Api.Controllers
{
	[Route("/courses")]
	public class CoursesController : BaseController
	{
		public CoursesController(ILogger logger, WebCourseManager courseManager)
			: base(logger, courseManager)
		{
		}
		
		[HttpGet("{courseId}")]
		public IActionResult CourseInfo(string courseId)
		{
			var course = courseManager.FindCourse(courseId);
			if (course == null)
				return Json(new { status = "error", message = $"Course {courseId} not found" });
			
			return Json(new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Units = course.Units.Select(BuildUnitInfo).ToList()
			});
		}

		private static UnitInfo BuildUnitInfo(Unit unit)
		{
			return new UnitInfo
			{
				Id = unit.Id,
				Title = unit.Title,
				Slides = unit.Slides.Select(BuildSlideInfo).ToList()
			};
		}

		private static SlideInfo BuildSlideInfo(Slide slide)
		{
			return new SlideInfo
			{
				Id = slide.Id,
				Title = slide.Title,
				Url = slide.Url,
				MaxScore = slide.MaxScore,
				Type = GetSlideType(slide)
			};
		}

		private static SlideType GetSlideType(Slide slide)
		{
			switch (slide)
			{
				case ExerciseSlide _:
					return SlideType.Exercise;
				case QuizSlide _:
					return SlideType.Quiz;
				default:
					return SlideType.Lesson;
			}
		}
	}
}