using System.Web.Mvc;
using System.Web.Routing;

namespace uLearn.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Course",
				url: "Course/{courseId}/{action}/{slideIndex}",
				defaults: new { controller = "Course", action = "Slide", slideIndex = 0 }
			);
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}",
				defaults: new { controller = "Home", action = "Index" }
			);
		}
	}
}
