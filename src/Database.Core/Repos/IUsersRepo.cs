using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.AspNetCore.Identity;

namespace Database.Repos
{
	public interface IUsersRepo
	{
		ApplicationUser FindUserById(string id);
		Task<List<UserRolesInfo>> FilterUsers(UserSearchQueryModel query, UserManager<ApplicationUser> userManager, int limit=100);
		List<string> FilterUsersByNamePrefix(string namePrefix);
		Task<List<UserRolesInfo>> GetCourseInstructors(string courseId, UserManager<ApplicationUser> userManager, int limit = 50);
		Task<List<UserRolesInfo>> GetCourseAdmins(string courseId, UserManager<ApplicationUser> userManager, int limit = 50);
		List<string> GetSysAdminsIds(UserManager<ApplicationUser> userManager);
		Task ChangeTelegram(string userId, long chatId, string chatTitle);
		Task ConfirmEmail(string userId, bool isConfirmed = true);
		Task UpdateLastConfirmationEmailTime(ApplicationUser user);
		Task ChangeEmail(ApplicationUser user, string email);
		ApplicationUser GetUlearnBotUser();
		string GetUlearnBotUserId();
		Task CreateUlearnBotUserIfNotExists();
		List<ApplicationUser> FindUsersByUsernameOrEmail(string usernameOrEmail);
		IEnumerable<ApplicationUser> GetUsersByIds(IEnumerable<string> usersIds);
		Task DeleteUserAsync(ApplicationUser user);
	}
}