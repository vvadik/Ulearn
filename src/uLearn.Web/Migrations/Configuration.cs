using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Models;

namespace uLearn.Web.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<uLearn.Web.DataContexts.ULearnDb>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(uLearn.Web.DataContexts.ULearnDb context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//

			var userStore = new UserStore<ApplicationUser>();
			var manager = new UserManager<ApplicationUser>(userStore);

			var studentRole = new IdentityUserRole { Role = new IdentityRole(Roles.Student) };
			var user = new ApplicationUser { UserName = "user" };
			user.Roles.Add(studentRole);
			manager.Create(user, "asdasd");
		}
	}
}
