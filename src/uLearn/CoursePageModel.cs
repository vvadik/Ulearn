using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace uLearn
{
	public class CoursePageModel
	{
		public Course Course;
		public Slide Slide;
		public int SlideIndex;
		public int NextSlideIndex;
		public int PrevSlideIndex;
		public bool HasNextSlide { get { return NextSlideIndex < Course.Slides.Length; } }
		public bool HasPrevSlide { get { return PrevSlideIndex >= 0; } }
		public bool IsPassedTask { get; set; }
		public string LatestAcceptedSolution { get; set; }
		public string Rate {get; set;}
		public HashSet<string> VisitedSlide { get; set; }
		public HashSet<string> SolvedSlide { get; set; }
		public HashSet<string> PassedQuiz { get; set; }
		public Dictionary<string, List<string>> AnswersToQuizes { get; set; }
		public bool IsSlideWithAcceptedSolutions { get; set; }
	}
}