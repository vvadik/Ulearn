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

		private IQueryable<UserRole> ToQueryableUserRoles()
		{
			var all = db.UserRoles
				.GroupBy(x => x.UserId + x.CourseId + x.Role)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(x => x.FirstOrDefault())
				.Where(x => x != null && (!x.IsEnabled.HasValue || x.IsEnabled.Value));
			return all;
		}

		private IEnumerable<UserRole> ToOriginalUserRoles()
		{
			var userRolesByUsers = new Dictionary<string, Dictionary<CourseRole, UserRole>>();
			var userRoles = db.UserRoles.ToList();
			userRoles.Reverse();
			foreach (var userRole in userRoles)
			{
				if (!userRolesByUsers.ContainsKey(userRole.UserId))
				{
					userRolesByUsers[userRole.UserId] = new Dictionary<CourseRole, UserRole>();
				}

				var roles = userRolesByUsers[userRole.UserId];
				if (!roles.ContainsKey(userRole.Role))
				{
					roles[userRole.Role] = userRole;
				}
			}

			var result = new List<UserRole>();
			foreach (var pair in userRolesByUsers)
			{
				var currentUserRoles = pair.Value;
				result.AddRange(currentUserRoles
					.Select(x => x.Value)
					.Where(x => !x.IsEnabled.HasValue || x.IsEnabled.Value));
			}

			return result;
		}

		public Dictionary<string, List<CourseRole>> GetRolesByUsers(string courseId)
		{
			var userRoles = ToQueryableUserRoles();
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
			var userRoles = ToQueryableUserRoles();
			return userRoles
				.Where(role => role.UserId == userId)
				.GroupBy(role => role.CourseId)
				.ToDictionary(g => g.Key, g => g.Select(role => role.Role).Min(), StringComparer.OrdinalIgnoreCase);
		}

		public async Task<bool> ToggleRole(string courseId, string userId, CourseRole role, string grantedById)
		{
			var userRole = db.UserRoles.Where(x => x.UserId == userId && x.Role == role && x.CourseId == courseId).ToList().LastOrDefault();
			bool isEnabled;
			if (userRole != null && (!userRole.IsEnabled.HasValue || userRole.IsEnabled.Value))
				isEnabled = false;
			else
				isEnabled = true;
			db.UserRoles.Add(new UserRole
			{
				UserId = userId,
				CourseId = courseId,
				Role = role,
				IsEnabled = isEnabled,
				GrantedById = grantedById,
				GrantTime = DateTime.Now
			});

			await db.SaveChangesAsync();

			return isEnabled;
		}

		public List<string> GetListOfUsersWithCourseRole(CourseRole? courseRole, string courseId, bool includeHighRoles = false)
		{
			if (!courseRole.HasValue)
				return null;

			var usersQuery = ToQueryableUserRoles();
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

			var usersQuery = ToQueryableUserRoles();
			if (courseId != null)
				usersQuery = usersQuery.Where(userRole => userRole.CourseId == courseId);
			return usersQuery.Select(userRole => userRole.UserId).Distinct().ToList();
		}

		public Dictionary<string, Dictionary<CourseRole, List<string>>> GetCoursesForUsers()
		{
			return ToQueryableUserRoles().GroupBy(userRole => userRole.UserId)
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

		public async Task<Dictionary<string, List<UserRole>>> GetUserRolesHistoryByCourseIds(string userId)
		{
			var result = new Dictionary<string, List<UserRole>>();
			var groupedByCourseId = db.UserRoles.Where(x => x.UserId == userId).GroupBy(x => x.CourseId);
			foreach (var userRoles in groupedByCourseId)
			{
				result[userRoles.Key] = userRoles.ToList();
			}

			return result;
		}
	}
}