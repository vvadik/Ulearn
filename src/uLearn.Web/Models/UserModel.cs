using System.Collections.Generic;
using Database;

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
		public Dictionary<string, ICoursesAccessListModel> CoursesAccess { get; set; }
	}

	public interface ICoursesAccessListModel {}

	public class SingleCourseAccessModel : ICoursesAccessListModel
	{
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}

	public class CourseAccessModel
	{
		public string CourseId { get; set; }
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}

	public class ManyCourseAccessModel : ICoursesAccessListModel
	{
		public List<CourseAccessModel> CoursesAccesses { get; set; }
	}
}