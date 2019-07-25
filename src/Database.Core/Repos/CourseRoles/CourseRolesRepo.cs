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
		private IQueryable<CourseRole> ToQueryableUserRoles(string userId)
		{
			var all = db.CourseRoles
				.Where(x => x.UserId == userId)
				.OrderByDescending(e => e.Id)
				.GroupBy(x => x.Role)
				.Select(x => x.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value));
			return all;
		}
		private IEnumerable<CourseRole> ToOriginalCourseRoles()
		{
			var userRolesByUsers = new Dictionary<string, Dictionary<CourseRoleType, CourseRole>>();
			var userRoles = db.CourseRoles.ToList();
			userRoles.Reverse();
			foreach (var userRole in userRoles)
			{
				if (!userRolesByUsers.ContainsKey(userRole.UserId))
				{
					userRolesByUsers[userRole.UserId] = new Dictionary<CourseRoleType, CourseRole>();
				}

				var roles = userRolesByUsers[userRole.UserId];
				if (!roles.ContainsKey(userRole.Role))
				{
					roles[userRole.Role] = userRole;
				}
			}

			var result = new List<CourseRole>();
			foreach (var pair in userRolesByUsers)
			{
				var currentUserRoles = pair.Value;
				result.AddRange(currentUserRoles
					.Select(x => x.Value)
					.Where(x => !x.IsEnabled.HasValue || x.IsEnabled.Value));
			}

			return result;
		}
			
		public async Task<Dictionary<string, CourseRoleType>> GetRolesAsync(string userId)
		{
			return await ToQueryableUserRoles(userId)
				.GroupBy(role => role.CourseId)
				.ToDictionaryAsync(g => g.Key, g => g.Select(role => role.Role).Min());
		}

		public async Task<bool> ToggleRoleAsync(string courseId, string userId, CourseRoleType roleType,string grantedById)
		{
			var userRoles = await db.CourseRoles.ToListAsync();

			var userRole = userRoles.LastOrDefault(u =>  u.Role == roleType && u.CourseId == courseId);
			bool isEnabled;
			if (userRole != null && (!userRole.IsEnabled.HasValue || userRole.IsEnabled.Value))
				isEnabled = false;
			else
				isEnabled = true;
			db.CourseRoles.Add(new CourseRole()
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

			return  await ToQueryableUserRoles(userId).Where(r =>  r.CourseId == courseId && r.Role <= minCourseRoleType).AnyAsync();
		}

		public async Task<bool> HasUserAccessToAnyCourseAsync(string userId, CourseRoleType minCourseRoleType)
		{
			var user = await usersRepo.FindUserByIdAsync(userId).ConfigureAwait(false);
			if (usersRepo.IsSystemAdministrator(user))
				return true;

			return  await ToQueryableUserRoles(userId).Where(r =>  r.Role <= minCourseRoleType).AnyAsync();
		}

		public async Task<List<string>> GetCoursesWhereUserIsInRoleAsync(string userId, CourseRoleType minCourseRoleType)
		{
			var roles =  await ToQueryableUserRoles(userId).Where(r =>r.Role <= minCourseRoleType).ToListAsync();
			return roles.Select(r => r.CourseId).ToList();
		}

		public async Task<List<string>> GetUsersWithRoleAsync(string courseId, CourseRoleType minCourseRoleType)
		{
			var a = await db.CourseRoles
				.Where(r => r.CourseId == courseId)
				.OrderByDescending(e => e.Id)
				.GroupBy(x => x.UserId + x.Role)
				.Select(gr => gr.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value))
				.Where(r => r.Role <= minCourseRoleType)
				.Select(r => r.UserId)
				.ToListAsync();
				return a
				.Distinct()
				.ToList();
		}
	}
}