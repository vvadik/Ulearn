using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class UnitStatisticPageModel
	{
		public string CourseId { get; set; }
		public string UnitName { get; set; }
		public UserQuestion[] Questions { get; set; }
		public SlideRateStats[] SlideRateStats { get; set; }
		public Slide[] Slides { get; set; }
		public List<UserInfo> UsersInfo { get; set; }
	}

	public class SlideRateStats
	{
		public string SlideId { get; set; }
		public string SlideTitle { get; set; }
		public int NotUnderstand { get; set; }
		public int Good { get; set; }
		public int Trivial { get; set; }

	}

	public class UserInfo
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string UserGroup { get; set; }
		public UserSlideInfo[] SlidesSlideInfo { get; set; }
	}

	public class UserSlideInfo
	{
		public bool IsVisited { get; set; }
		public bool IsExerciseSolved { get; set; }
		public int AttemptsCount { get; set; }
		public bool IsQuizPassed { get; set; }
		public double QuizPercentage { get; set; }
	}
}
