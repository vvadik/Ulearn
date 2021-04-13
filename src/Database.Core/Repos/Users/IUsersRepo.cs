using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;

namespace Database.Repos.Users
{
	public interface IUsersRepo
	{
		[ItemCanBeNull]
		Task<ApplicationUser> FindUserByIdAsync(string userId);
		Task<List<string>> GetSysAdminsIdsAsync();
		Task ChangeTelegram(string userId, long? chatId, string chatTitle);
		Task ConfirmEmail(string userId, bool isConfirmed = true);
		Task UpdateLastConfirmationEmailTime(ApplicationUser user);
		Task ChangeEmail(ApplicationUser user, string email);
		Task<ApplicationUser> GetUlearnBotUser();
		Task<string> GetUlearnBotUserId();
		Task CreateUlearnBotUserIfNotExistsAsync();
		List<ApplicationUser> FindUsersByUsernameOrEmail(string usernameOrEmail);
		Task<List<ApplicationUser>> GetUsersByIdsAsync(IEnumerable<string> usersIds);
		Task DeleteUserAsync(ApplicationUser user);
		bool IsSystemAdministrator(ApplicationUser user);
		Task<bool> IsSystemAdministrator(string userId);
		Task<List<string>> GetUserIdsWithLmsRoleAsync(LmsRoleType lmsRole);
		Task<List<string>> FindUsersBySocialProviderKeyAsync(string providerKey);
	}
}