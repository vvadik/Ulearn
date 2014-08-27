using System.Collections;
using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class UnitStatisticPageModel
	{
		public Slide[] Slides { get; set; }
		public Dictionary<string, UserInfo> Table { get; set; }//string - userId
		public string CourseId { get; set; }
		public string UnitName { get; set; }
		public UserQuestion[] Questions { get; set; }
		public SlideRate[] Rates { get; set; }
	}

	public class UserInfo
	{
		public string UserGroup { get; set; }
		public UserInfoInSlide[] SlidesInfo { get; set; }
	}

	public class UserInfoInSlide
	{
		public bool IsVisited { get; set; }
		public bool IsExerciseSolved { get; set; }
		public int AttemptsNumber { get; set; }
		public bool IsQuizPassed { get; set; }
		public int QuizSuccessful { get; set; }
	}
}
