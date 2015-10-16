using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class TocModelBuilder
	{
		private Func<Slide, string> getSlideUrl;
		private Func<Slide, int> getSlideScore;
		private readonly Course course;
		private readonly int currentSlideIndex;
		public Func<string, string> GetUnitStatisticsUrl;
		public Func<string, string> GetUnitInstructionNotesUrl;
		public Func<Slide, bool> IsSolved = s => false;
		public Func<Slide, bool> IsVisited = s => false;
		public Func<string, bool> IsUnitVisible = u => true;
		public bool IsInstructor;

		public TocModelBuilder(Func<Slide, string> getSlideUrl, Func<Slide, int> getSlideScore, Course course, int currentSlideIndex)
		{
			this.getSlideUrl = getSlideUrl;
			this.getSlideScore = getSlideScore;
			this.course = course;
			this.currentSlideIndex = currentSlideIndex;
		}

		public TocModel CreateTocModel()
		{
			return new TocModel(course, CreateUnits());
		}

		private TocUnitModel[] CreateUnits()
		{
			return course.Slides.GroupBy(s => s.Info.UnitName).Where(g => IsUnitVisible(g.Key)).Select(g => CreateUnit(g.Key, g.ToList())).ToArray();
		}

		private TocUnitModel CreateUnit(string unitName, List<Slide> slides)
		{
			var pages = slides.Select(CreatePage).ToList();
			if (IsInstructor)
			{
				if (course.FindInstructorNote(unitName) != null)
					pages.Add(new TocPageInfo
					{
						Url = GetUnitInstructionNotesUrl(unitName),
						Name = "Заметки преподавателю",
						PageType = TocPageType.InstructorNotes,
					});
				pages.Add(new TocPageInfo
				{
					Url = GetUnitStatisticsUrl(unitName),
					Name = "Статистика и успеваемость",
					PageType = TocPageType.Statistics,
				});
			}
			return new TocUnitModel
			{
				IsCurrent = slides.Any(s => s.Index == currentSlideIndex),
				UnitName = unitName,
				Pages = pages
			};
		}

		private TocPageInfo CreatePage(Slide slide)
		{
			var page = new TocPageInfo();
			page.Url = getSlideUrl(slide);
			page.Name = slide.Title;
			page.ShouldBeSolved = slide.ShouldBeSolved;
			page.MaxScore = slide.MaxScore;
			page.Score = getSlideScore(slide);
			page.IsCurrent = slide.Index == currentSlideIndex;
			page.IsSolved = IsSolved(slide);
			page.IsVisited = IsVisited(slide);
			page.PageType = GetPageType(slide);
			return page;
		}

		private TocPageType GetPageType(Slide slide)
		{
			if (slide is QuizSlide) return TocPageType.Quiz;
			if (slide is ExerciseSlide) return TocPageType.Exercise;
			return TocPageType.Theory;
		}
	}
}