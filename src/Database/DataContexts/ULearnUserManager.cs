using Database.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Database.DataContexts
{
	public class ULearnUserManager : UserManager<ApplicationUser>
	{
		public ULearnUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
			UserValidator = new UserValidator<ApplicationUser>(this)
			{
				AllowOnlyAlphanumericUserNames = false
			};
		}

		public ULearnUserManager(ULearnDb db)
			: this(new UserStore<ApplicationUser>(db))
		{
		}
	}
}