using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using uLearn;

namespace Database.Repos.Groups
{
	public interface IGroupsRepo
	{
		Task<Group> CreateGroupAsync(
			string courseId,
			string name,
			string ownerId,
			bool isManualCheckingEnabled=false,
			bool isManualCheckingEnabledForOldSolutions=false,
			bool canUsersSeeGroupProgress=true,
			bool defaultProhibitFurtherReview=true,
			bool isInviteLinkEnabled=true);

		Task<Group> CopyGroupAsync(Group group, string courseId, string newOwnerId = "");

		Task<Group> ModifyGroupAsync(
			int groupId,
			string newName,
			bool newIsManualCheckingEnabled,
			bool newIsManualCheckingEnabledForOldSolutions,
			bool newDefaultProhibitFurtherReview,
			bool newCanUsersSeeGroupProgress);

		Task ChangeGroupOwnerAsync(int groupId, string newOwnerId);
		Task<Group> ArchiveGroupAsync(int groupId, bool isArchived);
		Task DeleteGroupAsync(int groupId);
		Task<Group> FindGroupByIdAsync(int groupId, bool noTracking=false);
		Task<Group> FindGroupByInviteHashAsync(Guid hash, bool noTracking=false);
		Task<List<Group>> GetCourseGroupsAsync(string courseId, bool includeArchived=false);
		Task<List<Group>> GetMyGroupsFilterAccessibleToUserAsync(string courseId, string userId, bool includeArchived = false);
		Task EnableInviteLinkAsync(int groupId, bool isEnabled);
		Task<List<string>> GetUsersIdsForAllCourseGroupsAsync(string courseId);
		Task<Dictionary<string, List<int>>> GetUsersGroupsIdsAsync(string courseId, IEnumerable<string> usersIds);
		Task<List<int>> GetUserGroupsIdsAsync(string courseId, string userId);
		Task<List<Group>> GetUserGroupsAsync(string courseId, string userId);
		Task<bool> IsManualCheckingEnabledForUserAsync(Course course, string userId);
		Task<bool> GetDefaultProhibitFurtherReviewForUserAsync(string courseId, string userId, string instructorId);
		Task EnableAdditionalScoringGroupsForGroupAsync(int groupId, IEnumerable<string> scoringGroupsIds);
		Task<List<EnabledAdditionalScoringGroup>> GetEnabledAdditionalScoringGroupsAsync(string courseId);
		Task<List<EnabledAdditionalScoringGroup>> GetEnabledAdditionalScoringGroupsForGroupAsync(int groupId);
	}
}