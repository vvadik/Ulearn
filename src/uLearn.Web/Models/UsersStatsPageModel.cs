using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class UsersStatsPageModel
	{
		public string CourseId;
		public Dictionary<string, SortedDictionary<string, int>> UserStats;
		public List<string> UnitNamesInOrdered;
	}
}