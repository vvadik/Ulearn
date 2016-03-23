using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
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

		public ULearnUserManager()
			: this(new UserStore<ApplicationUser>(new ULearnDb()))
		{
		}
	}
}