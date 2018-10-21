using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos.CourseRoles
{
	public class CourseRoleUsersFilter : ICourseRoleUsersFilter
	{
		private readonly UlearnDb db;

		public CourseRoleUsersFilter(UlearnDb db)
		{
			this.db = db;
		}

		public Task<List<string>> GetListOfUsersWithCourseRoleAsync(CourseRoleType? courseRoleType, string courseId, bool includeHighRoles=false)
		{
			if (!courseRoleType.HasValue)
				return null;

			var userRoles = db.UserRoles.AsQueryable();
			userRoles = includeHighRoles
				? userRoles.Where(userRole => userRole.Role <= courseRoleType)
				: userRoles.Where(userRole => userRole.Role == courseRoleType);

			if (!string.IsNullOrEmpty(courseId))
				userRoles = userRoles.Where(userRole => userRole.CourseId == courseId);
			return userRoles.Select(user => user.UserId).Distinct().ToListAsync();
		}
		
		public Task<List<string>> GetListOfUsersByPrivilegeAsync(bool onlyPrivileged, string courseId)
		{
			if (!onlyPrivileged)
				return null;

			var userRoles = db.UserRoles.AsQueryable();
			if (courseId != null)
				userRoles = userRoles.Where(userRole => userRole.CourseId == courseId);
			return userRoles.Select(userRole => userRole.UserId).Distinct().ToListAsync();
		}
	}
}