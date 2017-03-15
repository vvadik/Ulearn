using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class GroupsFilterViewModel
	{
		public string CourseId { get; set; }

		public List<string> SelectedGroupsIds { get; set; }

		public List<Group> Groups { get; set; }
	}
}