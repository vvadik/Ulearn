using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class SlideInfo
	{
		public string FileName { get; private set; }
		public string UnitName { get; private set; }
		public string CourseName { get; private set; }

		public SlideInfo(string fileName, string unitName, string courseName)
		{
			FileName = fileName;
			CourseName = courseName;
			UnitName = unitName;
		}
	}
}
