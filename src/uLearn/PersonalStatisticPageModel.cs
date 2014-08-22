namespace uLearn
{
	public class PersonalStatisticPageModel
	{
		public string CourseId;
		public PersonalStatisticsInSlide[] PersonalStatistics;
	}

	public class PersonalStatisticsInSlide
	{
		public string UnitName{ get; set; }
		public string SlideTitle{ get; set; }
		public int SlideIndex { get; set; }
		public bool IsVisited { get; set; }
		public bool IsSolved { get; set; }
		public string UserRate { get; set; }
		public bool IsExercise { get; set; }
		public bool IsQuiz { get; set; }
		public int HintsCountOnSlide { get; set; }
		public int HintUsedPercent { get; set; }
	}
}
