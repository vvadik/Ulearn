using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Common;

namespace Database.Repos.Groups
{
	public interface IGroupAccessesRepo
	{
		Task<GroupAccess> GrantAccessAsync(int groupId, string userId, GroupAccessType accessType, string grantedById);
		Task<bool> CanRevokeAccessAsync(int groupId, string userId, ClaimsPrincipal revokedBy);
		Task<List<GroupAccess>> RevokeAccessAsync(int groupId, string userId);
		Task<List<GroupAccess>> GetGroupAccessesAsync(int groupId);
		Task<DefaultDictionary<int, List<GroupAccess>>> GetGroupAccessesAsync(IEnumerable<int> groupsIds);
		Task<bool> IsGroupVisibleForUserAsync(int groupId, ClaimsPrincipal user);
		Task<bool> IsGroupVisibleForUserAsync(Group group, ClaimsPrincipal user);
		Task<List<Group>> GetAvailableForUserGroupsAsync(string courseId, ClaimsPrincipal user, bool onlyArchived=false);
		Task<List<Group>> GetAvailableForUserGroupsAsync(List<string> coursesIds, ClaimsPrincipal user, bool onlyArchived=false);
		Task<bool> CanInstructorViewStudentAsync(ClaimsPrincipal instructor, string studentId);
		Task<List<string>> GetCoursesWhereUserCanSeeAllGroupsAsync(ClaimsPrincipal user, IEnumerable<string> coursesIds);
		Task<bool> HasUserAccessToGroupAsync(int groupId, string userId);
	}
}