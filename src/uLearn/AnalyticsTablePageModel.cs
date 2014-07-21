using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class AnalyticsTablePageModel
	{
		public Course Course;
		public Dictionary<string, AnalyticsTableInfo> TableInfo;
		public CoursePageModel CoursePageModel;
	}
}
