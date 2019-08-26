using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Common.Extensions;

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

		private IEnumerable<UserRole> GetActualUserRoles(string userId = null, string courseId = null)
		{
			IQueryable<UserRole> queryable = db.UserRoles;
			if (userId != null)
				queryable = queryable.Where(x => x.UserId == userId);
			if (courseId != null)
				queryable = queryable.Where(x => x.CourseId == courseId);
			var all = queryable.ToList()
				.GroupBy(x => x.UserId + x.CourseId + x.Role, StringComparer.OrdinalIgnoreCase)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(x => x.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value));
			return all;
		}

		public Dictionary<string, List<CourseRole>> GetRolesByUsers(string courseId)
		{
			var userRoles = GetActualUserRoles(courseId: courseId);
			return userRoles
				.GroupBy(role => role.UserId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(role => role.Role).Distinct().ToList()
				);
		}

		public Dictionary<string, CourseRole> GetRoles(string userId)
		{
			var userRoles = GetActualUserRoles(userId: userId);
			return userRoles
				.GroupBy(role => role.CourseId, StringComparer.OrdinalIgnoreCase)
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

			var usersRoles = GetActualUserRoles(courseId: courseId.NullIfEmptyOrWhitespace());
			usersRoles = includeHighRoles
				? usersRoles.Where(userRole => userRole.Role <= courseRole)
				: usersRoles.Where(userRole => userRole.Role == courseRole);
			return usersRoles.Select(user => user.UserId).Distinct().ToList();
		}

		public List<string> GetListOfUsersByPrivilege(bool onlyPrivileged, string courseId)
		{
			if (!onlyPrivileged)
				return null;

			var usersRoles = GetActualUserRoles(courseId: courseId);
			return usersRoles.Select(userRole => userRole.UserId).Distinct().ToList();
		}

		public Dictionary<string, Dictionary<CourseRole, List<string>>> GetCoursesForUsers()
		{
			return GetActualUserRoles()
				.GroupBy(userRole => userRole.UserId)
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