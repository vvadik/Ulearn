using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class CourseRolesRepo : ICourseRolesRepo
	{
		private readonly UlearnDb db;
		private readonly IUsersRepo usersRepo;

		public CourseRolesRepo(UlearnDb db, IUsersRepo usersRepo)
		{
			this.db = db;
			this.usersRepo = usersRepo;
		}
			
		public Task<Dictionary<string, CourseRoleType>> GetRolesAsync(string userId)
		{
			return db.UserRoles
				.Where(role => role.UserId == userId)
				.GroupBy(role => role.CourseId)
				.ToDictionaryAsync(g => g.Key, g => g.Select(role => role.Role).Min());
		}

		public async Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRoleType roleType)
		{
			var userRole = await db.UserRoles.FirstOrDefaultAsync(u => u.UserId == userId && u.Role == roleType && u.CourseId == courseId).ConfigureAwait(false);
			if (userRole == null)
				db.UserRoles.Add(new UserRole
				{
					UserId = userId,
					CourseId = courseId,
					Role = roleType
				});
			else
				db.UserRoles.Remove(userRole);
			await db.SaveChangesAsync().ConfigureAwait(false);

			return userRole == null;
		}

		public async Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRoleType minCourseRoleType)
		{
			var user = await usersRepo.FindUserByIdAsync(userId).ConfigureAwait(false);
			if (usersRepo.IsSystemAdministrator(user))
				return true;
			
			return await db.UserRoles.Where(r => r.UserId == userId && r.CourseId == courseId && r.Role <= minCourseRoleType).AnyAsync().ConfigureAwait(false);
		}

		public async Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRoleType minCourseRoleType)
		{
			var user = await usersRepo.FindUserByIdAsync(userId).ConfigureAwait(false);
			if (usersRepo.IsSystemAdministrator(user))
				return true;
			
			return await db.UserRoles.Where(r => r.UserId == userId && r.Role <= minCourseRoleType).AnyAsync().ConfigureAwait(false);
		}
	}
}