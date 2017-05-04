using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CourseManager;
using Database.Extensions;
using Database.Models;

namespace uLearn.Web.Extensions
{
	public static class UserExtensions
	{
		public static IEnumerable<string> GetControllableCoursesId(this IPrincipal principal)
		{
			if (!principal.IsSystemAdministrator())
				return principal.GetCoursesIdFor(CourseRole.Instructor);
			var courseManager = WebCourseManager.Instance;
			return courseManager.GetCourses().Select(course => course.Id);
		}
	}
}