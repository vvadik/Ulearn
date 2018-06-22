using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Database
{
	public class InitialDataCreator
	{
		private readonly UlearnDb db;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UlearnUserManager userManager;
		private readonly UsersRepo usersRepo;

		public InitialDataCreator(
			UlearnDb db,
			RoleManager<IdentityRole> roleManager,
			UlearnUserManager userManager,
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
			var sysAdminRole = LmsRoles.SysAdmin.ToString();
			if (! await db.Roles.AnyAsync(r => r.Name == sysAdminRole))
			{
				await roleManager.CreateAsync(new IdentityRole(sysAdminRole));
			}

			if (! await db.Users.AnyAsync(u => u.UserName == "user"))
			{
				var user = new ApplicationUser { UserName = "user", FirstName = "User", LastName = "" };
				await userManager.CreateAsync(user, "asdasd");
			}
			if (! await db.Users.AnyAsync(u => u.UserName == "admin"))
			{
				var user = new ApplicationUser { UserName = "admin", FirstName = "System Administrator", LastName = "" };
				await userManager.CreateAsync(user, "fullcontrol");
				await userManager.AddToRoleAsync(user, sysAdminRole);
			}

			await usersRepo.CreateUlearnBotUserIfNotExists();

			await db.SaveChangesAsync();
		}
	}
}