using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common.Extensions;

namespace Database.Repos.Groups
{
	/* It's helper class for building lists of groups with this student.
	   I.e. if student is a member of group A, B, C, D and E, but current instructor has access only to groups A, B, D, E, then
	   he will see "A, B, D, ..." as a groups list for this student. */
	public class UsersGroupsGetter
	{
		private readonly UlearnDb db;
		private readonly GroupAccessesRepo groupAccessesRepo;

		public UsersGroupsGetter(UlearnDb db, GroupAccessesRepo groupAccessesRepo)
		{
			this.db = db;
			this.groupAccessesRepo = groupAccessesRepo;
		}

		public async Task<Dictionary<string, List<Group>>> GetUsersGroupsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false)
		{
			var coursesWhereUserCanSeeAllGroups = await groupAccessesRepo.GetCoursesWhereUserCanSeeAllGroupsAsync(currentUser, courseIds).ConfigureAwait(false);
			coursesWhereUserCanSeeAllGroups = coursesWhereUserCanSeeAllGroups.Select(c => c.ToLower()).ToList();
			var currentUserId = currentUser.GetUserId();

			var groupsWithAccess = new HashSet<int>(db.GroupAccesses.Where(a => a.UserId == currentUserId && a.IsEnabled).Select(a => a.GroupId));
			var userGroups = await db.GroupMembers
				.Where(m => userIds.Contains(m.UserId) && courseIds.Contains(m.Group.CourseId))
				.GroupBy(m => m.UserId)
				.ToDictionaryAsync(group => group.Key, group => group.ToList())
				.ConfigureAwait(false);
			var usersGroups = userGroups
				.ToDictionary(
					kv => kv.Key,
					kv => kv.Value.Select(m => m.Group)
						.Distinct()
						.Where(g => (g.OwnerId == currentUserId || groupsWithAccess.Contains(g.Id) || coursesWhereUserCanSeeAllGroups.Contains(g.CourseId.ToLower())) && !g.IsDeleted)
						.Where(g => onlyArchived ? g.IsArchived : ! g.IsArchived)
						.OrderBy(g => g.OwnerId != currentUserId)
						.Take(maxCount)
						.ToList()
				);

			return usersGroups;
		}

		public async Task<Dictionary<string, List<string>>> GetUsersGroupsNamesAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false)
		{
			var usersGroups = await GetUsersGroupsAsync(courseIds, userIds, currentUser, maxCount + 1, onlyArchived).ConfigureAwait(false);
			return usersGroups.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Select((g, idx) => idx >= maxCount ? "..." : g.Name).ToList());
		}

		public async Task<Dictionary<string, List<int>>> GetUsersGroupsIdsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false)
		{
			var usersGroups = await GetUsersGroupsAsync(courseIds, userIds, currentUser, maxCount, onlyArchived).ConfigureAwait(false);
			return usersGroups.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Select(g => g.Id).ToList());
		}

		public async Task<Dictionary<string, string>> GetUsersGroupsNamesAsStringsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false)
		{
			var usersGroups = await GetUsersGroupsNamesAsync(courseIds, userIds, currentUser, maxCount, onlyArchived).ConfigureAwait(false);
			return usersGroups.ToDictionary(kv => kv.Key, kv => string.Join(", ", kv.Value));
		}

		public Task<Dictionary<string, string>> GetUsersGroupsNamesAsStringsAsync(string courseId, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false)
		{
			return GetUsersGroupsNamesAsStringsAsync(new List<string> { courseId }, userIds, currentUser, maxCount, onlyArchived);
		}

		public async Task<string> GetUserGroupsNamesAsStringAsync(List<string> courseIds, string userId, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false)
		{
			var usersGroups = await GetUsersGroupsNamesAsStringsAsync(courseIds, new List<string> { userId }, currentUser, maxCount, onlyArchived).ConfigureAwait(false);
			return usersGroups.GetOrDefault(userId, "");
		}

		public Task<string> GetUserGroupsNamesAsStringAsync(string courseId, string userId, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false)
		{
			return GetUserGroupsNamesAsStringAsync(new List<string> { courseId }, userId, currentUser, maxCount, onlyArchived);
		}
	}
}