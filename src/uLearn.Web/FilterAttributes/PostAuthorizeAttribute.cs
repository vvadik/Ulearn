using System.Web.Mvc;
using System.Web.Routing;

namespace uLearn.Web.FilterAttributes
{
	public class PostAuthorizeAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			var request = filterContext.HttpContext.Request;
			if (request.HttpMethod == "POST" && request.UrlReferrer != null)
			{
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login", returnUrl = request.UrlReferrer.PathAndQuery }));
				return;
			}
			base.HandleUnauthorizedRequest(filterContext);
		}
	}
}