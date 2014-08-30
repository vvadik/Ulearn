using System;
using System.IO;
using System.Web.Hosting;

namespace uLearn.Web
{
	public class WebCourseManager : CourseManager
	{
		public WebCourseManager() 
			: base(new DirectoryInfo(GetAppPath()))
		{
		}

		private static string GetAppPath()
		{
			return HostingEnvironment.ApplicationPhysicalPath ?? "..";
		}

		public static CourseManager Instance = new WebCourseManager();
	}
}