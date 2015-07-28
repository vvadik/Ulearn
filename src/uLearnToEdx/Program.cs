using System;
using System.IO;
using System.Linq;
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
			const string uLearnCourseName = "Linq";
			const string videoJson = "";
			const string exerciesUrl = "https://192.168.33.1:44300/Course/{0}/LtiSlide/{1}";
			const string solutionsUrl = "https://192.168.33.1:44300/Course/{0}/AcceptedAlert/{1}";

			Console.WriteLine("Downloading {0}...", course);
			DownloadManager.Download(hostname, port, email, password, organization, course, time, time + ".tar.gz");
			
			Console.WriteLine("Extracting {0}...", course);
			ArchiveManager.ExtractTar(time + ".tar.gz", ".");
			var edxCourse = EdxCourse.Load(time);

			Console.WriteLine("Loading {0}", uLearnCourseName);
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse(string.Format("{0}.zip", uLearnCourseName));
			var ulearnCourse = cm.GetCourses().Single();

			Console.WriteLine("Patching {0} with slides from {1}...", course, uLearnCourseName);
			edxCourse.PatchSlides(time, ulearnCourse.Id, ulearnCourse.Slides, exerciesUrl, solutionsUrl);
			
			Console.WriteLine("Creating {0}.tar.gz...", time);
			ArchiveManager.CreateTar(time + ".tar.gz", time);
			
			Console.WriteLine("Uploading {0}.tar.gz...", course);
			DownloadManager.Upload(hostname, port, email, password, organization, course, time, time + ".tar.gz");
		}
	}
}
