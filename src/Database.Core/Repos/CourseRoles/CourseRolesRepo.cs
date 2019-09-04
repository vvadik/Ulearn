using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos.Users;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos.CourseRoles
{
	/* This repo is fully migrated to .NET Core and EF Core */
	public class CourseRolesRepo : ICourseRolesRepo
	{
		private readonly UlearnDb db;
		private readonly IUsersRepo usersRepo;

		public CourseRolesRepo(UlearnDb db, IUsersRepo usersRepo)
		{
			this.db = db;
			this.usersRepo = usersRepo;
		}

		private async Task<List<CourseRole>>GetUserRoles(string userId)
		{
			var userCourseRoles = await db.CourseRoles.Where(x => x.UserId == userId).ToListAsync().ConfigureAwait(false);
			return userCourseRoles
				.GroupBy(x => x.Role + x.CourseId, StringComparer.OrdinalIgnoreCase)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(x => x.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value))
				.ToList();
		}

		public async Task<Dictionary<string, CourseRoleType>> GetRolesAsync(string userId)
		{
			return (await GetUserRoles(userId))
				.GroupBy(role => role.CourseId, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(g => g.Key, g => g.Select(role => role.Role).Min());
		}

		public async Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRoleType roleType, string grantedById)
		{
			var userRoles = await db.CourseRoles.ToListAsync();

			var userRole = userRoles.LastOrDefault(u => u.UserId == userId && u.Role == roleType && string.Equals(u.CourseId, courseId, StringComparison.OrdinalIgnoreCase));
			bool isEnabled;
			if (userRole != null && (!userRole.IsEnabled.HasValue || userRole.IsEnabled.Value))
				isEnabled = false;
			else
				isEnabled = true;
			db.CourseRoles.Add(new CourseRole
			{
				UserId = userId,
				CourseId = courseId,
				Role = roleType,
				IsEnabled = isEnabled,
				GrantedById = grantedById,
				GrantTime = DateTime.Now
			});

			await db.SaveChangesAsync();

			return isEnabled;
		}

		public async Task<bool> HasUserAccessToCourseAsync(string userId, string courseId, CourseRoleType minCourseRoleType)
		{
			var user = await usersRepo.FindUserByIdAsync(userId).ConfigureAwait(false);
			if (usersRepo.IsSystemAdministrator(user))
				return true;

			return (await GetUserRoles(userId)).Any(r => string.Equals(r.CourseId, courseId, StringComparison.OrdinalIgnoreCase) && r.Role <= minCourseRoleType);
		}

		public async Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRoleType minCourseRoleType)
		{
			var user = await usersRepo.FindUserByIdAsync(userId).ConfigureAwait(false);
			if (usersRepo.IsSystemAdministrator(user))
				return true;

			return (await GetUserRoles(userId)).Any(r => r.Role <= minCourseRoleType);
		}

		public async Task<List<string>> GetCoursesWhereUserIsInRoleAsync(string userId, CourseRoleType minCourseRoleType)
		{
			var roles = (await GetUserRoles(userId)).Where(r => r.Role <= minCourseRoleType).ToList();
			return roles.Select(r => r.CourseId).ToList();
		}

		public async Task<List<string>> GetUsersWithRoleAsync(string courseId, CourseRoleType minCourseRoleType)
		{
			return (await db.CourseRoles
				.Where(r => r.CourseId == courseId)
				.OrderByDescending(e => e.Id)
				.ToListAsync())
				.GroupBy(x => x.UserId + x.Role)
				.Select(gr => gr.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value))
				.Where(r => r.Role <= minCourseRoleType)
				.Select(r => r.UserId)
				.Distinct()
				.ToList();
		}
	}
}