using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Core.Extensions;

namespace Database.DataContexts
{
	public class UserRolesRepo
	{
		private readonly ULearnDb db;

		public UserRolesRepo(ULearnDb db)
		{
			this.db = db;
		}

		public UserRolesRepo()
			: this(new ULearnDb())
		{
		}

		private IQueryable<UserRole> GetActualUserRolesQueryable()
		{
			var all = db.UserRoles
				.GroupBy(x => x.UserId + x.CourseId + x.Role)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(x => x.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value));
			return all;
		}

		public Dictionary<string, List<CourseRole>> GetRolesByUsers(string courseId)
		{
			var userRoles = GetActualUserRolesQueryable();
			return userRoles
				.Where(role => role.CourseId == courseId)
				.GroupBy(role => role.UserId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(role => role.Role).Distinct().ToList()
				);
		}

		public Dictionary<string, CourseRole> GetRoles(string userId)
		{
			var userRoles = GetActualUserRolesQueryable();
			return userRoles
				.Where(role => role.UserId == userId)
				.GroupBy(role => role.CourseId)
				.ToDictionary(g => g.Key, g => g.Select(role => role.Role).Min(), StringComparer.OrdinalIgnoreCase);
		}

		public async Task<bool> ToggleRole(string courseId, string userId, CourseRole role, string grantedById, string comment)
		{
			courseId = courseId.ToLower();
			var userRole = db.UserRoles.Where(x => x.UserId == userId && x.Role == role && x.CourseId == courseId).ToList().LastOrDefault();
			bool isEnabled;
			if (userRole != null && (!userRole.IsEnabled.HasValue || userRole.IsEnabled.Value))
				isEnabled = false;
			else
				isEnabled = true;
			var record = new UserRole
			{
				UserId = userId,
				CourseId = courseId,
				Role = role,
				IsEnabled = isEnabled,
				GrantedById = grantedById,
				GrantTime = DateTime.Now.ToUniversalTime(),
				Comment = comment
			};
			db.UserRoles.Add(record);

			await db.SaveChangesAsync();

			return isEnabled;
		}

		public List<string> GetListOfUsersWithCourseRole(CourseRole? courseRole, string courseId, bool includeHighRoles = false)
		{
			if (!courseRole.HasValue)
				return null;

			var usersQuery = GetActualUserRolesQueryable();
			usersQuery = includeHighRoles
				? usersQuery.Where(userRole => userRole.Role <= courseRole)
				: usersQuery.Where(userRole => userRole.Role == courseRole);

			if (!string.IsNullOrEmpty(courseId))
				usersQuery = usersQuery.Where(userRole => userRole.CourseId == courseId);
			return usersQuery.Select(user => user.UserId).Distinct().ToList();
		}

		public List<string> GetListOfUsersByPrivilege(bool onlyPrivileged, string courseId)
		{
			if (!onlyPrivileged)
				return null;

			var usersQuery = GetActualUserRolesQueryable();
			if (courseId != null)
				usersQuery = usersQuery.Where(userRole => userRole.CourseId == courseId);
			return usersQuery.Select(userRole => userRole.UserId).Distinct().ToList();
		}

		public Dictionary<string, Dictionary<CourseRole, List<string>>> GetCoursesForUsers()
		{
			return GetActualUserRolesQueryable().GroupBy(userRole => userRole.UserId)
				.ToDictionary(
					g => g.Key,
					g => g
						.GroupBy(userRole => userRole.Role)
						.ToDictionary(
							gg => gg.Key,
							gg => gg
								.Select(userRole => userRole.CourseId.ToLower())
								.ToList()
						)
				);
		}

		public async Task<List<UserRole>> GetUserRolesHistory(string userId)
		{
			return await db.UserRoles.Where(x => x.UserId == userId).ToListAsync();
		}

		public async Task<List<UserRole>> GetUserRolesHistoryByCourseId(string userId, string courseId)
		{
			courseId = courseId.ToLower();
			return await db.UserRoles.Where(x => x.UserId == userId && x.CourseId == courseId).ToListAsync();
		}
	}
}