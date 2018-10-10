using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Groups
{
	public interface IUsersGroupsGetter
	{
		Task<Dictionary<string, List<Group>>> GetUsersGroupsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false);
		Task<Dictionary<string, List<string>>> GetUsersGroupsNamesAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false);
		Task<Dictionary<string, List<int>>> GetUsersGroupsIdsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false);
		Task<Dictionary<string, string>> GetUsersGroupsNamesAsStringsAsync(List<string> courseIds, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount=3, bool onlyArchived=false);
		Task<Dictionary<string, string>> GetUsersGroupsNamesAsStringsAsync(string courseId, IEnumerable<string> userIds, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false);
		Task<string> GetUserGroupsNamesAsStringAsync(List<string> courseIds, string userId, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false);
		Task<string> GetUserGroupsNamesAsStringAsync(string courseId, string userId, ClaimsPrincipal currentUser, int maxCount = 3, bool onlyArchived=false);
	}
}