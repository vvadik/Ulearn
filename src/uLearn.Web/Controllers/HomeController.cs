using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ULearnDb()));

		public ActionResult Index()
		{
			if (User.Identity.GetUserId() == null || ControllerUtils.HasPassword(userManager, User))
				return View();
			return RedirectToAction("Manage", "Account");
		}
	}
}