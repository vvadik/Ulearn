using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.CourseRoles
{
	public interface ICourseRolesRepo
	{
		Task<Dictionary<string, CourseRoleType>> GetRolesAsync(string userId);
		Task<CourseRoleType> GetRoleAsync(string userId, string courseId);
		Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRoleType roleType, string grantedById, string comment);
		Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRoleType minCourseRoleType);
		Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRoleType minCourseRoleType);
		Task<List<string>> GetCoursesWhereUserIsInRoleAsync(string userId, CourseRoleType minCourseRoleType);
		Task<List<string>> GetCoursesWhereUserIsInStrictRoleAsync(string userId, CourseRoleType courseRoleType);
		Task<List<string>> GetUsersWithRoleAsync(string courseId, CourseRoleType minCourseRoleType);
	}
}