using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class UnitStatisticPageModel
	{
		public Slide[] Slides { get; set; }
		public Dictionary<string, UserInfo> Table { get; set; }//string - userId
		public string CourseId { get; set; }
		public string UnitName { get; set; }
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
