using System.Web.Mvc;

namespace uLearn.Web.Controllers
{
	public class SlidesBarController : Controller
	{
		[ChildActionOnly]
		public ActionResult ShowSlides()
		{
			return View("Slides");
		}
	}
}