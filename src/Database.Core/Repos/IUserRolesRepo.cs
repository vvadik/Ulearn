using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IUserRolesRepo
	{
		Task<Dictionary<string, CourseRole>> GetRolesAsync(string userId);
		Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRole role);
		List<string> GetListOfUsersWithCourseRole(CourseRole? courseRole, string courseId, bool includeHighRoles = false);
		List<string> GetListOfUsersByPrivilege(bool onlyPrivileged, string courseId);
		Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRole minCourseRole);
		Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRole minCourseRole);
	}
}