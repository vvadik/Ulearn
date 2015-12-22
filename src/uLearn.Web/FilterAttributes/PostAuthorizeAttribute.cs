using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using uLearn.Web.Models;

namespace uLearn.Web.FilterAttributes
{
	public class PostAuthorizeAttribute : AuthorizeAttribute
	{
		private CourseRoles? minAccessLevel;

		public PostAuthorizeAttribute(string roles = null)
		{
			if (roles != null)
				Roles = roles;
		}

		public PostAuthorizeAttribute(CourseRoles minAccessLevel, string roles = null)
			: this(roles)
		{
			this.minAccessLevel = minAccessLevel;
		}

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

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			var user = httpContext.User;
			if (!user.Identity.IsAuthenticated)
				return false;

			if (!minAccessLevel.HasValue && rolesSplit.Length == 0)
				return true;

			if (rolesSplit.Length > 0 && rolesSplit.Any(user.IsInRole))
				return true;

			if (!minAccessLevel.HasValue)
				return false;

			var courseIds = httpContext.Request.QueryString.Get("courseId");
			if (courseIds == null)
				return user.HasAccess(minAccessLevel.Value);

			var courseId = courseIds.Split(',').FirstOrDefault();
			return user.HasAccessFor(courseId, minAccessLevel.Value);
		}

		private string[] rolesSplit = new string[0];

		public new string Roles
		{
			set { rolesSplit = SplitString(value); }
		}

		public new string Users
		{
			set { throw new NotImplementedException(); }
		}

		private static string[] SplitString(string original)
		{
			if (String.IsNullOrEmpty(original))
			{
				return new string[0];
			}

			return original
				.Split(',')
				.Select(piece => piece.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.ToArray();
		}
	}
}