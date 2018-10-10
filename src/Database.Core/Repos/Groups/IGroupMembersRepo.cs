using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Groups
{
	public interface IGroupMembersRepo
	{
		Task<List<ApplicationUser>> GetGroupMembersAsUsersAsync(int groupId);
		Task<List<GroupMember>> GetGroupMembersAsync(int groupId);
		Task<List<GroupMember>> GetGroupsMembersAsync(IEnumerable<int> groupsIds);
		Task<GroupMember> AddUserToGroupAsync(int groupId, string userId);
		Task<GroupMember> RemoveUserFromGroupAsync(int groupId, string userId);
		Task<List<GroupMember>> RemoveUsersFromGroupAsync(int groupId, List<string> userIds);
		Task<List<GroupMember>> CopyUsersFromOneGroupToAnotherAsync(int fromGroupId, int toGroupId, List<string> userIds);
	}
}