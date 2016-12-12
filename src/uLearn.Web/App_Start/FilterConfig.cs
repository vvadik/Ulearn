using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace uLearn.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			var requireHttps = Convert.ToBoolean(WebConfigurationManager.AppSettings["ulearn.requireHttps"] ?? "true");
			if (requireHttps)
				filters.Add(new RequireHttpsForCloudFlareAttribute());
			filters.Add(new AntiForgeryTokenFilter());
		}
	}

	public class AntiForgeryTokenFilter : FilterAttribute, IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			if (!(filterContext.Exception is HttpAntiForgeryException))
				return;
			filterContext.Result = new RedirectResult("/");
			filterContext.ExceptionHandled = true;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequireHttpsForCloudFlareAttribute : RequireHttpsAttribute
	{
		private readonly string xSchemeHeaderName = "X-Scheme";

		/* Additionally view X-Scheme header. If it equals to "HTTPS", continue work */
		protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
		{
			if (string.Equals(filterContext.HttpContext.Request.Headers[xSchemeHeaderName], "HTTPS", StringComparison.OrdinalIgnoreCase))
				return;
			if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && 
				!string.Equals(filterContext.HttpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("Require HTTPS");
			var url = "https://" + filterContext.HttpContext.Request.Url?.Host + filterContext.HttpContext.Request.RawUrl;
			filterContext.Result = new RedirectResult(url);
		}
	}
}