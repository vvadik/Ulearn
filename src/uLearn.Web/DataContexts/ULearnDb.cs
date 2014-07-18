using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Migrations;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class ULearnDb : IdentityDbContext<ApplicationUser>
	{
		public ULearnDb()
			: base("DefaultConnection")
		{
			Database.SetInitializer(new CreateDatabaseIfNotExists<ULearnDb>());
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<ULearnDb, Configuration>());

		}

		public DbSet<UserSolution> UserSolutions { get; set; }
		public DbSet<UserQuestion> UserQuestions { get; set; }
	}
}