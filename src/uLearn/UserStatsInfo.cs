using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class UserStatsInfo
	{
		public Dictionary<string, UnitStat> PercentAcceptedSolutionsPerUnit;

		public UserStatsInfo()
		{
			PercentAcceptedSolutionsPerUnit = new Dictionary<string, UnitStat>();
		}
	}

	public class UnitStat
	{
		public int MaxCount { get; set; }
		public int Count { get; set; }

		public UnitStat()
		{
			MaxCount = 1;
			Count = 1;
		}

		public void AddMaxCount()
		{
			MaxCount++;
		}

		public void AddCount()
		{
			Count++;
		}
	}
}
