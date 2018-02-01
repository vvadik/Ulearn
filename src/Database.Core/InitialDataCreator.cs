using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Microsoft.AspNetCore.Identity;

namespace Database
{
	public class InitialDataCreator
	{
		private readonly UlearnDb db;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly ULearnUserManager userManager;
		private readonly UsersRepo usersRepo;

		public InitialDataCreator(
			UlearnDb db,
			RoleManager<IdentityRole> roleManager,
			ULearnUserManager userManager,
			UsersRepo usersRepo
		)
		{
			this.db = db;
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.usersRepo = usersRepo;
		}

		public async Task CreateInitialDataAsync()
		{
			await roleManager.CreateAsync(new IdentityRole(LmsRoles.SysAdmin.ToString()));

			if (!db.Users.Any(u => u.UserName == "user"))
			{
				var user = new ApplicationUser { UserName = "user", FirstName = "User", LastName = "" };
				await userManager.CreateAsync(user, "asdasd");
			}
			if (!db.Users.Any(u => u.UserName == "admin"))
			{
				var user = new ApplicationUser { UserName = "admin", FirstName = "System Administrator", LastName = "" };
				await userManager.CreateAsync(user, "fullcontrol");
				await userManager.AddToRoleAsync(user, LmsRoles.SysAdmin.ToString());
			}
			
			usersRepo.CreateUlearnBotUserIfNotExists().Wait();

			await db.SaveChangesAsync();
		}
	}
}