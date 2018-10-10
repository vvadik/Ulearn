using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos.Groups
{
	public class GroupMembersRepo : IGroupMembersRepo
	{
		private readonly UlearnDb db;
		private readonly IManualCheckingsForOldSolutionsAdder manualCheckingsForOldSolutionsAdder;
		private readonly IGroupsRepo groupsRepo;

		public GroupMembersRepo(UlearnDb db, IManualCheckingsForOldSolutionsAdder manualCheckingsForOldSolutionsAdder, IGroupsRepo groupsRepo)
		{
			this.db = db;
			this.manualCheckingsForOldSolutionsAdder = manualCheckingsForOldSolutionsAdder;
			this.groupsRepo = groupsRepo;
		}
		
		public Task<List<ApplicationUser>> GetGroupMembersAsUsersAsync(int groupId)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => m.GroupId == groupId && !m.User.IsDeleted).Select(m => m.User).ToListAsync();
		}

		public Task<List<GroupMember>> GetGroupMembersAsync(int groupId)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => m.GroupId == groupId && !m.User.IsDeleted).ToListAsync();
		}
		
		public Task<List<GroupMember>> GetGroupsMembersAsync(IEnumerable<int> groupsIds)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => groupsIds.Contains(m.GroupId) && !m.User.IsDeleted).ToListAsync();
		}
		
		public async Task<GroupMember> AddUserToGroupAsync(int groupId, string userId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			
			var groupMember = new GroupMember
			{
				GroupId = groupId,
				UserId = userId,
				AddingTime = DateTime.Now,
			};
			using (var transaction = db.Database.BeginTransaction())
			{
				/* Don't add member if it's already exists */
				var existsMember = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
				if (existsMember != null)
					return null;

				db.GroupMembers.Add(groupMember);
				await db.SaveChangesAsync().ConfigureAwait(false);

				transaction.Commit();
			}
			
			if (group.IsManualCheckingEnabledForOldSolutions)
				await manualCheckingsForOldSolutionsAdder.AddManualCheckingsForOldSolutionsAsync(group.CourseId, userId).ConfigureAwait(false);

			return groupMember;
		}

		public async Task<GroupMember> RemoveUserFromGroupAsync(int groupId, string userId)
		{
			var member = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
			if (member != null)
				db.GroupMembers.Remove(member);

			await db.SaveChangesAsync().ConfigureAwait(false);
			
			return member;
		}
		
		public async Task<List<GroupMember>> RemoveUsersFromGroupAsync(int groupId, List<string> userIds)
		{
			var members = db.GroupMembers.Where(m => m.GroupId == groupId && userIds.Contains(m.UserId)).ToList();
			db.GroupMembers.RemoveRange(members);

			await db.SaveChangesAsync().ConfigureAwait(false);
			
			return members;
		}
		
		public async Task<List<GroupMember>> CopyUsersFromOneGroupToAnotherAsync(int fromGroupId, int toGroupId, List<string> userIds)
		{
			var membersUserIds = db.GroupMembers.Where(m => m.GroupId == fromGroupId && userIds.Contains(m.UserId)).Select(m => m.UserId).ToList();
			var newMembers = new List<GroupMember>();
			foreach (var memberUserId in membersUserIds)
				newMembers.Add(await AddUserToGroupAsync(toGroupId, memberUserId).ConfigureAwait(false));
			
			return newMembers;
		}
	}
}