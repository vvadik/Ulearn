using System;
using System.Collections.Generic;
using System.Linq;
using Database.Models;
using Ulearn.Core.Courses;
using uLearn.Web.Controllers;

namespace uLearn.Web.Models
{
	public class UserCourseHistoryModel
	{
		public ApplicationUser User { get; set; }
		public Course Course;
		public List<UserGrantModel> UserGrantsHistory;


		public UserCourseHistoryModel(ApplicationUser user, Course course,  List<RoleGrantModel> rolesHistory, List<AccessGrantModel> accessesHistory)
		{
			User = user;
			Course = course;
			UserGrantsHistory = MergeUserRolesAndAccessesGrants(rolesHistory, accessesHistory);
		}

		private List<UserGrantModel> MergeUserRolesAndAccessesGrants( List<RoleGrantModel> rolesHistory, List<AccessGrantModel> accessesHistory)
		{
			return rolesHistory
				.Select(x => x.ToUserGrantModel())
				.Concat(accessesHistory.Select(x => x.ToUserGrantModel()))
				.OrderByDescending(x=>x.GrantTimeUtc)
				.ToList();
		}
	}


	public class UserGrantModel
	{
		public string GrantedBy { get; set; }
		public string Grant { get; set; }
		public Type GrantType { get; set; }
		public DateTime GrantTimeUtc { get; set; }
		public bool IsEnabled { get; set; }
		public string Comment { get; set; }
	}
}