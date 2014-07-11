using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class ULearnDb : IdentityDbContext<ApplicationUser>
	{
		public ULearnDb()
			: base("DefaultConnection")
		{
		}

		public DbSet<UserSolution> UserSolutions { get; set; }
	}
}