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
		public Dictionary<string, List<UserGrantModel>> UserGrantsHistory;


		public UserCourseHistoryModel(ApplicationUser user, Course course, Dictionary<string, List<RoleGrantModel>> rolesHistory, Dictionary<string, List<AccessGrantModel>> accessesHistory)
		{
			User = user;
			Course = course;
			UserGrantsHistory = MergeUserRolesAndAccessesGrants(rolesHistory, accessesHistory);
		}

		private Dictionary<string, List<UserGrantModel>> MergeUserRolesAndAccessesGrants(Dictionary<string, List<RoleGrantModel>> rolesHistory, Dictionary<string, List<AccessGrantModel>> accessesHistory)
		{
			var courseIds = rolesHistory.Keys.Concat(accessesHistory.Keys);
			var result = new Dictionary<string, List<UserGrantModel>>();
			foreach (var courseId in courseIds)
			{
				result[courseId] = new List<UserGrantModel>();
				if (rolesHistory.TryGetValue(courseId, out var roleGrants))
					foreach (var roleGrant in roleGrants)
					{
						result[courseId].Add(roleGrant.ToUserGrantModel());
					}

				if (accessesHistory.TryGetValue(courseId, out var accessGrants))
					foreach (var accessGrant in accessGrants)
					{
						result[courseId].Add(accessGrant.ToUserGrantModel());
					}

				result[courseId] = result[courseId].OrderByDescending(x => x.GrantTimeUtc).ToList();
			}

			return result;
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