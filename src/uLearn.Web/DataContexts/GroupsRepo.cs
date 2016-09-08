using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class GroupsRepo
	{
		private readonly ULearnDb db;

		public GroupsRepo() : this(new ULearnDb())
		{

		}

		public GroupsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public bool CanUserSeeAllCourseGroups(IPrincipal user, string courseId)
		{
			return user.HasAccessFor(courseId, CourseRole.CourseAdmin);
		}

		public async Task<Group> CreateGroup(string courseId, string name, string ownerId, bool isPublic=false)
		{
			var group = new Group
			{
				CourseId = courseId,
				Name = name,
				IsPublic = isPublic,
				OwnerId = ownerId
			};
			db.Groups.Add(group);
			await db.SaveChangesAsync();

			return group;
		}

		public async Task<Group> ModifyGroup(int groupId, string newName, bool newIsPublic)
		{
			var group = FindGroupById(groupId);
			group.Name = newName;
			group.IsPublic = newIsPublic;
			await db.SaveChangesAsync();

			return group;
		}

		public async Task RemoveGroup(int groupId)
		{
			var group = db.Groups.Find(groupId);
			group.IsDeleted = true;
			await db.SaveChangesAsync();
		}

		public async Task<bool> AddUserToGroup(int groupId, string userId)
		{
			var groupMember = new GroupMember
			{
				GroupId = groupId,
				UserId = userId
			};
			using (var transaction = db.Database.BeginTransaction())
			{
				/* Don't add member if it's already exists */
				var existsMember = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
				if (existsMember != null)
					return false;

				db.GroupMembers.Add(groupMember);
				await db.SaveChangesAsync();

				transaction.Commit();
			}
			return true;
		}

		public async Task<bool> AddUserToGroup(int groupId, ApplicationUser user)
		{
			return await AddUserToGroup(groupId, user.Id);
		}

		public async Task RemoveUserFromGroup(int groupId, string userId)
		{
			var member = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
			if (member != null)
				db.GroupMembers.Remove(member);
			await db.SaveChangesAsync();
		}

		public Group FindGroupById(int groupId)
		{
			return db.Groups.FirstOrDefault(g => g.Id == groupId && ! g.IsDeleted);
		}

		public Group FindGroupByInviteHash(Guid hash)
		{
			return db.Groups.FirstOrDefault(g => g.InviteHash == hash && !g.IsDeleted && g.IsInviteLinkEnabled);
		}

		public List<Group> GetGroups(string courseId)
		{
			return db.Groups.Where(g => g.CourseId == courseId && ! g.IsDeleted).ToList();
		}

		public bool IsGroupAvailableForUser(int groupId, IPrincipal user)
		{
			var group = FindGroupById(groupId);
			/* Course admins can see all groups */
			if (CanUserSeeAllCourseGroups(user, group.CourseId))
				return true;

			if (!user.HasAccessFor(group.CourseId, CourseRole.Instructor))
				return false;

			var userId = user.Identity.GetUserId();
			return !group.IsDeleted && (group.OwnerId == userId || group.IsPublic);
		}

		public List<Group> GetAvailableForUserGroups(string courseId, IPrincipal user)
		{
			/* Course admins can see all groups */
			if (CanUserSeeAllCourseGroups(user, courseId))
				return GetGroups(courseId);

			if (!user.HasAccessFor(courseId, CourseRole.Instructor))
				return new List<Group>();

			var userId = user.Identity.GetUserId();
			return db.Groups.Where(g => g.CourseId == courseId && !g.IsDeleted && (g.OwnerId == userId || g.IsPublic)).OrderBy(g => g.OwnerId == userId).ToList();
		}

		public List<ApplicationUser> GetGroupMembers(int groupId)
		{
			return db.GroupMembers.Where(m => m.GroupId == groupId).Select(m => m.User).ToList();
		}

		public Dictionary<string, List<string>> GetUsersGroupsNames(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var canSeeAllGroups = courseIds.ToDictionary(c => c, c => CanUserSeeAllCourseGroups(currentUser, c));
			var currentUserId = currentUser.Identity.GetUserId();

			var usersGroups = db.GroupMembers
				.Where(m => userIds.Contains(m.UserId) && courseIds.Contains(m.Group.CourseId))
				.GroupBy(m => m.UserId)
				.ToDictionary(group => group.Key, group => group.ToList())
				.ToDictionary(
					kv => kv.Key,
					kv => kv.Value.Select(m => m.Group)
						.Distinct()
						.Where(g => (g.OwnerId == currentUserId || g.IsPublic || canSeeAllGroups[g.CourseId]) && !g.IsDeleted)
						.OrderBy(g => g.OwnerId != currentUserId)
						.Take(maxCount + 1)
						.Select((g, idx) => idx >= maxCount ? "..." : g.Name)
						.ToList()
				);

			return usersGroups;
		}

		public Dictionary<string, string> GetUsersGroupsNamesAsStrings(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroupsNames(courseIds, userIds, currentUser, maxCount);
			return usersGroups.ToDictionary(kv => kv.Key, kv => string.Join(", ", kv.Value));
		}

		public Dictionary<string, string> GetUsersGroupsNamesAsStrings(string courseId, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			return GetUsersGroupsNamesAsStrings(new List<string> { courseId }, userIds, currentUser, maxCount);
		}

		public string GetUserGroupsNamesAsString(List<string> courseIds, string userId, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroupsNamesAsStrings(courseIds, new List<string> { userId } , currentUser, maxCount);
			return usersGroups.GetOrDefault(userId, "");
		}

		public string GetUserGroupsNamesAsString(string courseId, string userId, IPrincipal currentUser, int maxCount = 3)
		{
			return GetUserGroupsNamesAsString(new List<string> { courseId }, userId, currentUser, maxCount);
		}

		public async Task EnableGroupInviteLink(int groupId, bool isEnabled)
		{
			var group = db.Groups.Find(groupId);
			group.IsInviteLinkEnabled = isEnabled;
			await db.SaveChangesAsync();
		}
	}
}