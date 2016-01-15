using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class UserRolesInfo
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string GroupName { get; set; }
		public List<string> Roles { get; set; }
	}

	public class UserListModel
	{
		public List<UserModel> Users { get; set; }
		public bool IsCourseAdmin { get; set; }
		public bool ShowDangerEntities { get; set; }
	}

	public class UserModel
	{
		public UserModel(UserRolesInfo userRoles)
		{
			UserName = userRoles.UserName;
			UserId = userRoles.UserId;
			GroupName = userRoles.GroupName;
		}

		public string UserId { get; set; }
		public string UserName { get; set; }
		public string GroupName { get; set; }
		public Dictionary<string, ICoursesAccessListModel> CoursesAccess { get; set; }
	}

	public interface ICoursesAccessListModel {}

	public class OneOptionCourseAccessModel : ICoursesAccessListModel
	{
		public string CourseId { get; set; }
		public bool HasAccess { get; set; }
		public string ToggleUrl { get; set; }
	}

	public class ManyOptionsCourseAccessModel : ICoursesAccessListModel
	{
		public List<OneOptionCourseAccessModel> Courses { get; set; }
	}
}