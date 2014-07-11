using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using uLearn.CSharp;

namespace uLearn.Web.Models
{
	public class CourseManager
	{
		private static readonly ResourceLoader Resources = new ResourceLoader(typeof (CourseManager));
		public static CourseManager AllCourses = LoadAllCourses();

		private readonly List<Course> courses = new List<Course>();


		private static CourseManager LoadAllCourses()
		{
			var result = new CourseManager();
			result.AddCourse("Linq", "Linq");
			result.AddCourse("BasicProgramming", "BasicProgramming");
			return result;
		}

		private void AddCourse(string courseId, string courseTitle)
		{
			courses.Add(LoadCourse(courseId, courseTitle));
		}

		private static Course LoadCourse(string courseId, string courseTitle)
		{
			var slideFiles = Resources.EnumerateResourcesFrom("uLearn.Web.Courses." + courseId)
				.OrderBy(f => f.Filename)
				.ToList();
			var slides = slideFiles.Select(f =>
			{
				var ans = SlideParser.ParseCode(Encoding.UTF8.GetString(f.GetContent()));
				ans.Info.FileName = f.Filename;
				return ans;
			}).ToArray();
			AddLocationInfo(slides);
			return new Course(courseId, courseTitle, slides);
		}

		public static Dictionary<string,LocationSlideInfo> GetSlidesInfo()
		{
			//HostingEnvironment.MapPath("~/Courses");
			var prefix = HostingEnvironment.MapPath("~/Courses"); ;
			var blocksDirectories = Directory.GetDirectories(prefix);
			var insertTable = new Dictionary<string, LocationSlideInfo>();
			foreach (var blockPath in blocksDirectories)
			{
				foreach (var unitName in Directory.GetDirectories(blockPath))
				{
					foreach (var slideName in Directory.GetFiles(unitName))
					{
						if (Path.GetExtension(slideName) != ".cs") continue;
						var info = new LocationSlideInfo("", "", "", "");
						info.FileName = Path.GetFileName(slideName);
						info.BlockName = File.ReadAllText(blockPath + "\\BlockName.txt");
						info.UnitName = File.ReadAllText(unitName + "//UnitName.txt");
						info.DesiredTitle = ExtractTitle(slideName);
						insertTable.Add(info.FileName, info);
					}
				}
			}
			return insertTable;
		}

		public static void AddLocationInfo(Slide[] slides)
		{
			var table = GetSlidesInfo();
			foreach (var slide in slides)
			{
				slide.Info = table[slide.Info.FileName];
			}
		}

		public static string ExtractTitle(string path)
		{

			var attribute = new Regex(@"\[(Title.*?)\]");
			var argument = new Regex(@"\((.*?)\)");
			var quad = attribute.Match(File.ReadAllText(path)).ToString();
			var ans = argument.Match(quad).ToString();
			if (ans != "")
				return ans.Substring(2,ans.Length-4);
			return "undefined";
		}

		public Course GetCourse(string courseId)
		{
			Course course = courses.FirstOrDefault(c => c.Id == courseId);
			if (course == null) throw new Exception(string.Format("Course {0} not found", courseId));
			return course;
		}
	}
}