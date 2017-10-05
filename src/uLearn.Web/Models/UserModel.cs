using System.Collections.Generic;
using Database;
using Database.Models;

namespace uLearn.Web.Models
{
	public class UserListModel
	{
		public List<UserModel> Users { get; set; }
		public Dictionary<string, string> UsersGroups { get; set; }
		public bool IsCourseAdmin { get; set; }
		public bool ShowDangerEntities { get; set; }
	}

	public class UserModel
	{
		public UserModel(UserRolesInfo userRoles)
		{
			UserName = userRoles.UserName;
			UserId = userRoles.UserId;
			UserVisibleName = userRoles.UserVisibleName;
		}

		public string UserId { get; private set; }
		public string UserName { get; private set; }
		public string UserVisibleName { get; private set; }
		public Dictionary<string, ICoursesRolesListModel> CourseRoles { get; set; }
		public Dictionary<CourseAccessType, CourseAccessModel> CourseAccesses { get; set; }
	}

	public interface ICoursesRolesListModel
	{
	}

	public class SingleCourseRolesModel : ICoursesRolesListModel
	{
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}

	public class CourseRoleModel
	{
		public string CourseId { get; set; }
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}

	public class ManyCourseRolesModel : ICoursesRolesListModel
	{
		public List<CourseRoleModel> CourseRoles { get; set; }
	}

	public class CourseAccessModel
	{
		public string CourseId { get; set; }
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}
}