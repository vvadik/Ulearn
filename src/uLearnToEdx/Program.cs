using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using uLearn;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	public class Program
	{
		public static void Main(string[] args)
		{
			const string hostname = "192.168.33.10";
			const int port = 18010;
			const string organization = "SKBKontur";
			const string course = "Linq101";
			const string time = "Linq101_2015";
			const string email = "staff@example.com";
			const string password = "edx";

			Downloader.Download(hostname, port, email, password, organization, course, time, "filename");
			Downloader.Upload(hostname, port, email, password, organization, course, time, "ForTests.tar.gz");
//
//						var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
//						cm.ReloadCourse("BasicProgramming.zip");
//						var course = cm.GetCourses().Single();
//						var converted = Converter.ToEdxCourse(course, "Kontur", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "192.168.33.1:44300", new Dictionary<string, string>());
//						converted.Save(course.Id);
//						ArchiveManager.CreateTar(course.Id + ".tar.gz", course.Id);
//						
//						EdxCourse
//							.Load("BasicProgramming")
//							.Save("BasicProgramming2");
//						ArchiveManager.CreateTar("BasicProgramming2.tar.gz", "BasicProgramming2");
		}
	}
}
