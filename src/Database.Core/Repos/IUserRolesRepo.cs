using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IUserRolesRepo
	{
		Task<Dictionary<string, CourseRole>> GetRolesAsync(string userId);
		Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRole role);
		Task<List<string>> GetListOfUsersWithCourseRoleAsync(CourseRole? courseRole, string courseId, bool includeHighRoles = false);
		Task<List<string>> GetListOfUsersByPrivilegeAsync(bool onlyPrivileged, string courseId);
		Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRole minCourseRole);
		Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRole minCourseRole);
	}
}