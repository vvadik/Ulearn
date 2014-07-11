using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class LocationSlideInfo
	{
		public string FileName { get; set; }
		public string DesiredTitle {get; set; }
		public string UnitName { get; set; }
		public string BlockName { get; set; }

		public LocationSlideInfo(string fileName, string desiredTitle, string unitName, string blockName)
		{
			DesiredTitle = desiredTitle;
			FileName = fileName;
			BlockName = blockName;
			UnitName = unitName;
		}
	}
}
