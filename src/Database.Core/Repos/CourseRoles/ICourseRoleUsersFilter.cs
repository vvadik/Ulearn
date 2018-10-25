using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.CourseRoles
{
	public interface ICourseRoleUsersFilter
	{
		Task<List<string>> GetListOfUsersWithCourseRoleAsync(CourseRoleType? courseRoleType, string courseId, bool includeHighRoles=false);
		Task<List<string>> GetListOfUsersByPrivilegeAsync(bool onlyPrivileged, string courseId);
	}
}