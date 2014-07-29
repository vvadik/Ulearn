using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class PersonalStatisticPageModel
	{
		public CoursePageModel CoursePageModel;
		public Dictionary<string, PersonalStatisticsInSlide> PersonalStatistics;
	}

	public class PersonalStatisticsInSlide
	{
		public bool IsVisited { get; set; }
		public bool IsSolved { get; set; }
		public string UserMark { get; set; }
		public bool IsExercise { get; set; }
	}
}
