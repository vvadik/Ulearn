using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.AspNetCore.Identity;

namespace Database.Repos
{
	public interface IUsersRepo
	{
		Task<ApplicationUser> FindUserByIdAsync(string userId);
		Task<List<UserRolesInfo>> FindUsers(UserSearchQuery query, int limit = 100);
		List<string> FilterUsersByNamePrefix(string namePrefix);
		Task<List<UserRolesInfo>> GetCourseInstructors(string courseId, int limit = 50);
		Task<List<UserRolesInfo>> GetCourseAdmins(string courseId, int limit = 50);
		Task<List<string>> GetSysAdminsIdsAsync();
		Task ChangeTelegram(string userId, long chatId, string chatTitle);
		Task ConfirmEmail(string userId, bool isConfirmed = true);
		Task UpdateLastConfirmationEmailTime(ApplicationUser user);
		Task ChangeEmail(ApplicationUser user, string email);
		ApplicationUser GetUlearnBotUser();
		string GetUlearnBotUserId();
		Task CreateUlearnBotUserIfNotExistsAsync();
		List<ApplicationUser> FindUsersByUsernameOrEmail(string usernameOrEmail);
		IEnumerable<ApplicationUser> GetUsersByIds(IEnumerable<string> usersIds);
		Task DeleteUserAsync(ApplicationUser user);
	}
}