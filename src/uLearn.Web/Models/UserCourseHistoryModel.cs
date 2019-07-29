using System.Collections.Generic;
using Database.Models;
using Ulearn.Core.Courses;
using uLearn.Web.Controllers;

namespace uLearn.Web.Models
{
	public class UserCourseHistoryModel
	{
		public ApplicationUser User { get; set; }
		public Course Course;
		public Dictionary<string, List<RoleGrantModel>> UserRightsHistory;

		public UserCourseHistoryModel(ApplicationUser user, Course course, Dictionary<string, List<RoleGrantModel>> history)
		{
			User = user;
			Course = course;
			UserRightsHistory = history;
		}
		
	}
}