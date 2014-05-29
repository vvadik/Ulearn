using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Models;

namespace uLearn.Web.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			ContextKey = "uLearn.Web.Models.ApplicationDbContext";
		}

		protected override void Seed(ApplicationDbContext context)
		{
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
			userManager.Create(new ApplicationUser {UserName = "ad"}, "asdqwe");
		}
	}
}