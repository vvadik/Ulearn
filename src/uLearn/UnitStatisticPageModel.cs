using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class UnitStatisticPageModel
	{
		public CourseUnitModel Unit { get; set; }
		public Dictionary<string, UserInfoInSlide[]> Table { get; set; }
		public string CourseId { get; set; }

//string - userId
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
