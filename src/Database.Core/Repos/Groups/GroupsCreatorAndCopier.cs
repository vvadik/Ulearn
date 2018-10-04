using System;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ulearn.Common.Extensions;

namespace Database.Repos.Groups
{
	public class GroupsCreatorAndCopier
	{
		private readonly UlearnDb db;
		private readonly ILogger logger;
		private readonly UserRolesRepo userRolesRepo;

		public GroupsCreatorAndCopier(UlearnDb db, ILogger logger, UserRolesRepo userRolesRepo)
		{
			this.db = db;
			this.logger = logger;
			this.userRolesRepo = userRolesRepo;
		}

		public async Task<Group> CreateGroupAsync(
			string courseId,
			string name,
			string ownerId,
			bool isManualCheckingEnabled=false,
			bool isManualCheckingEnabledForOldSolutions=false,
			bool canUsersSeeGroupProgress=true,
			bool defaultProhibitFurtherReview=true,
			bool isInviteLinkEnabled=true)
		{
			logger.Information($"Создаю новую группу в курсе {courseId}: «{name}»");
			var group = new Group
			{
				CourseId = courseId,
				Name = name,
				OwnerId = ownerId,
				CreateTime = DateTime.Now,
				
				IsManualCheckingEnabled = isManualCheckingEnabled,
				IsManualCheckingEnabledForOldSolutions = isManualCheckingEnabledForOldSolutions,
				CanUsersSeeGroupProgress = canUsersSeeGroupProgress,
				DefaultProhibitFutherReview = defaultProhibitFurtherReview,
				
				InviteHash = Guid.NewGuid(),
				IsInviteLinkEnabled = isInviteLinkEnabled,
			};
			db.Groups.Add(group);
			await db.SaveChangesAsync().ConfigureAwait(false);

			return group;
		}
		
		/* Copy group from one course to another. Replace owner only if newOwnerId is not empty */
		public async Task<Group> CopyGroupAsync(Group group, string courseId, string newOwnerId="")
		{
			logger.Information($"Копирую группу «{group.Name}» (id={group.Id}) в курс {courseId}");
			
			var newGroup = await CopyGroupWithoutMembersAsync(group, courseId, newOwnerId).ConfigureAwait(false);
			await CopyGroupMembersAsync(group, newGroup).ConfigureAwait(false);
			await CopyGroupAccessesAsync(group, newGroup).ConfigureAwait(false);

			/* We can also copy group's scoring-group settings if they are in one course */
			if (group.CourseId.EqualsIgnoreCase(courseId))
			{
				await CopyEnabledAdditionalScoringGroupsAsync(group, newGroup).ConfigureAwait(false);
			}

			return newGroup;
		}

		private Task<Group> CopyGroupWithoutMembersAsync(Group group, string courseId, string newOwnerId)
		{
			var newName = group.Name;
			if (courseId == group.CourseId)
				newName += " (копия)";

			return CreateGroupAsync(
				courseId,
				newName,
				string.IsNullOrEmpty(newOwnerId) ? group.OwnerId : newOwnerId,
				group.IsManualCheckingEnabled,
				group.IsManualCheckingEnabledForOldSolutions,
				group.CanUsersSeeGroupProgress,
				group.DefaultProhibitFutherReview,
				group.IsInviteLinkEnabled
			);
		}

		private async Task CopyGroupMembersAsync(Group group, Group newGroup)
		{
			var members = group.NotDeletedMembers.Select(m => new GroupMember
			{
				UserId = m.UserId,
				GroupId = newGroup.Id,
				AddingTime = DateTime.Now,
			});
			db.GroupMembers.AddRange(members);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		private async Task CopyGroupAccessesAsync(Group group, Group newGroup)
		{
			var accesses = await db.GroupAccesses.Where(a => a.GroupId == group.Id && a.IsEnabled).ToListAsync().ConfigureAwait(false);
			var courseInstructorsIds = userRolesRepo.GetListOfUsersWithCourseRole(CourseRole.Instructor, newGroup.CourseId, includeHighRoles: true);
			foreach (var access in accesses)
			{
				if (! courseInstructorsIds.Contains(access.UserId))
					continue;
				db.GroupAccesses.Add(new GroupAccess
				{
					GroupId = newGroup.Id,
					UserId = access.UserId,
					AccessType = access.AccessType,
					GrantedById = access.GrantedById,
					GrantTime = DateTime.Now,
					IsEnabled = true,
				});
			}

			await db.SaveChangesAsync().ConfigureAwait(false);
		}
		
		private async Task CopyEnabledAdditionalScoringGroupsAsync(Group group, Group newGroup)
		{
			var enabledAdditionalScoringGroups = await db.EnabledAdditionalScoringGroups
				.Where(s => s.GroupId == group.Id)
				.Select(s => s.ScoringGroupId)
				.ToListAsync()
				.ConfigureAwait(false);
			
			foreach (var scoringGroupId in enabledAdditionalScoringGroups)
				db.EnabledAdditionalScoringGroups.Add(new EnabledAdditionalScoringGroup
				{
					GroupId = newGroup.Id,
					ScoringGroupId = scoringGroupId,
				});

			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}