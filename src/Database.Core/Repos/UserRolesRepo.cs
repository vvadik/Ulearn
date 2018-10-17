using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UserRolesRepo : IUserRolesRepo
	{
		private readonly UlearnDb db;

		public UserRolesRepo(UlearnDb db)
		{
			this.db = db;
		}
			
		public Task<Dictionary<string, CourseRole>> GetRolesAsync(string userId)
		{
			return db.UserRoles
				.Where(role => role.UserId == userId)
				.GroupBy(role => role.CourseId)
				.ToDictionaryAsync(g => g.Key, g => g.Select(role => role.Role).Min());
		}

		public async Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRole role)
		{
			var userRole = await db.UserRoles.FirstOrDefaultAsync(u => u.UserId == userId && u.Role == role && u.CourseId == courseId).ConfigureAwait(false);
			if (userRole == null)
				db.UserRoles.Add(new UserRole
				{
					UserId = userId,
					CourseId = courseId,
					Role = role
				});
			else
				db.UserRoles.Remove(userRole);
			await db.SaveChangesAsync().ConfigureAwait(false);

			return userRole == null;
		}

		public Task<List<string>> GetListOfUsersWithCourseRoleAsync(CourseRole? courseRole, string courseId, bool includeHighRoles=false)
		{
			if (!courseRole.HasValue)
				return null;

			var userRoles = db.UserRoles.AsQueryable();
			userRoles = includeHighRoles
				? userRoles.Where(userRole => userRole.Role <= courseRole)
				: userRoles.Where(userRole => userRole.Role == courseRole);

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

		public Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRole minCourseRole)
		{
			return db.UserRoles.Where(r => r.UserId == userId && r.CourseId == courseId && r.Role <= minCourseRole).AnyAsync();
		}

		public Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRole minCourseRole)
		{
			return db.UserRoles.Where(r => r.UserId == userId && r.Role <= minCourseRole).AnyAsync();
		}
	}
}