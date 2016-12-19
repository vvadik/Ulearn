using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using uLearn.Web.Extensions;
using uLearn.Web.Models;

namespace uLearn.Web.FilterAttributes
{
	public class ULearnAuthorizeAttribute : AuthorizeAttribute
	{
		public CourseRole MinAccessLevel = CourseRole.Student;

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
				throw new ArgumentNullException(nameof(httpContext));

			var user = httpContext.User;
			if (!user.Identity.IsAuthenticated)
				return false;

			if (MinAccessLevel == CourseRole.Student && rolesSplit.Length == 0)
				return true;

			if (rolesSplit.Length > 0 && rolesSplit.Any(user.IsInRole))
				return true;

			if (MinAccessLevel == CourseRole.Student)
				return false;

			var courseIds = httpContext.Request.Unvalidated.QueryString.GetValues("courseId");
			if (courseIds == null)
				return user.HasAccess(MinAccessLevel);

			if (courseIds.Length != 1)
				return false;

			return user.HasAccessFor(courseIds[0], MinAccessLevel);
		}

		private string[] rolesSplit = new string[0];

		public new string Roles
		{
			get { return string.Join(",", rolesSplit); }
			set { rolesSplit = SplitString(value); }
		}

		public new string Users
		{
			set { throw new NotSupportedException(); }
		}

		private static readonly char[] delims = { ',' };

		private static string[] SplitString(string original)
		{
			if (String.IsNullOrEmpty(original))
			{
				return new string[0];
			}

			return original
				.Split(delims)
				.Select(piece => piece.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.ToArray();
		}
	}
}