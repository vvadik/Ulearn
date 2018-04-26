using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using uLearn;
using Ulearn.Common;

namespace Database.Repos
{
	public class UsersRepo
	{
		private readonly UlearnDb db;
		private readonly ULearnUserManager userManager;
		private readonly UserRolesRepo userRolesRepo;

		public const string UlearnBotUsername = "ulearn-bot";

		public UsersRepo(
			UlearnDb db,
			ULearnUserManager userManager,
			UserRolesRepo userRolesRepo
		)
		{
			this.db = db;
			this.userManager = userManager;
			this.userRolesRepo = userRolesRepo;
		}

		public ApplicationUser FindUserById(string id)
		{
			var user = db.Users.Find(id);
			return user == null || user.IsDeleted ? null : user;
		}

		/* Pass limit=0 to disable limiting */
		public Task<List<UserRolesInfo>> FilterUsers(UserSearchQueryModel query, UserManager<ApplicationUser> userManager, int limit=100)
		{
			var role = db.Roles.FirstOrDefault(r => r.Name == query.Role);
			var users = db.Users.Where(u => !u.IsDeleted);
			if (!string.IsNullOrEmpty(query.NamePrefix))
			{
				var usersIds = GetUsersByNamePrefix(query.NamePrefix).Select(u => u.Id);
				users = users.Where(u => usersIds.Contains(u.Id));
			}
			return users
				.FilterByRole(role, userManager)
				.FilterByUserIds(
					userRolesRepo.GetListOfUsersWithCourseRole(query.CourseRole, query.CourseId, query.IncludeHighCourseRoles),
					userRolesRepo.GetListOfUsersByPrivilege(query.OnlyPrivileged, query.CourseId)
				)
				.GetUserRolesInfo(limit, userManager);
		}

		/* Pass limit=0 to disable limiting */
		public Task<List<UserRolesInfo>> GetCourseInstructors(string courseId, UserManager<ApplicationUser> userManager, int limit = 50)
		{
			return db.Users
				.Where(u => !u.IsDeleted)
				.FilterByUserIds(userRolesRepo.GetListOfUsersWithCourseRole(CourseRole.Instructor, courseId, includeHighRoles: true))
				.GetUserRolesInfo(limit, userManager);
		}

		/* Pass limit=0 to disable limiting */
		public Task<List<UserRolesInfo>> GetCourseAdmins(string courseId, UserManager<ApplicationUser> userManager, int limit = 50)
		{
			return db.Users
				.Where(u => !u.IsDeleted)	
				.FilterByUserIds(userRolesRepo.GetListOfUsersWithCourseRole(CourseRole.CourseAdmin, courseId, includeHighRoles: true))
				.GetUserRolesInfo(limit, userManager);
		}

		public List<string> GetSysAdminsIds(UserManager<ApplicationUser> userManager)
		{
			var role = db.Roles.FirstOrDefault(r => r.Name == LmsRoles.SysAdmin.ToString());
			if (role == null)
				return new List<string>();
			return db.Users.Where(u => ! u.IsDeleted).FilterByRole(role, userManager).Select(u => u.Id).ToList();
		}

		public async Task ChangeTelegram(string userId, long chatId, string chatTitle)
		{
			var user = FindUserById(userId);
			if (user == null)
				return;

			user.TelegramChatId = chatId;
			user.TelegramChatTitle = chatTitle;
			await db.SaveChangesAsync();
		}

		public async Task ConfirmEmail(string userId, bool isConfirmed = true)
		{
			var user = FindUserById(userId);
			if (user == null)
				return;

			user.EmailConfirmed = isConfirmed;
			await db.SaveChangesAsync();
		}

		private IQueryable<UserIdWrapper> GetUsersByNamePrefix(string name)
		{
			if (string.IsNullOrEmpty(name))
				return db.Users.Where(u => ! u.IsDeleted).Select(u => new UserIdWrapper(u.Id));
			
			var splittedName = name.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			var nameQuery = string.Join(" & ", splittedName.Select(s => "\"" + s.Trim().Replace("\"", "\\\"") + "*\""));
			return db.Users
				.FromSql("SELECT * FROM dbo.AspNetUsers WHERE IsDeleted = 0 AND CONTAINS([Names], {0})", nameQuery)
				.Select(u => new UserIdWrapper(u.Id));
		}

		public async Task UpdateLastConfirmationEmailTime(ApplicationUser user)
		{
			user.LastConfirmationEmailTime = DateTime.Now;
			await db.SaveChangesAsync();
		}

		public async Task ChangeEmail(ApplicationUser user, string email)
		{
			user.Email = email;
			user.EmailConfirmed = false;
			await db.SaveChangesAsync();
		}

		[NotNull]
		public ApplicationUser GetUlearnBotUser()
		{
			var user = db.Users.FirstOrDefault(u => u.UserName == UlearnBotUsername);
			if (user == null)
				throw new NotFoundException($"Ulearn bot user (username = {UlearnBotUsername}) not found");
			return user;
		}

		public string GetUlearnBotUserId()
		{
			var user = GetUlearnBotUser();
			return user.Id;
		}

		public async Task CreateUlearnBotUserIfNotExists()
		{
			var ulearnBotFound = await db.Users.AnyAsync(u => u.UserName == UlearnBotUsername).ConfigureAwait(false);
			if (! ulearnBotFound)
			{
				var user = new ApplicationUser
				{
					UserName = UlearnBotUsername,
					FirstName = "Ulearn",
					LastName = "bot",
					Email = "support@ulearn.me",
				};
				await userManager.CreateAsync(user, StringUtils.GenerateSecureAlphanumericString(10)).ConfigureAwait(false);

				await db.SaveChangesAsync().ConfigureAwait(false);
			}
		}

		public List<ApplicationUser> FindUsersByUsernameOrEmail(string usernameOrEmail)
		{
			return db.Users.Where(u => (u.UserName == usernameOrEmail || u.Email == usernameOrEmail) && ! u.IsDeleted).ToList();
		}

		public IEnumerable<ApplicationUser> GetUsersByIds(IEnumerable<string> usersIds)
		{
			return db.Users.Where(u => usersIds.Contains(u.Id) && ! u.IsDeleted);
		}
	}

	/* System.String is not available for table-valued functions so we need to create ComplexTyped wrapper */

	[ComplexType]
	public class UserIdWrapper
	{
		public UserIdWrapper(string userId)
		{
			Id = userId;
		}

		public string Id { get; set; }
	}
}