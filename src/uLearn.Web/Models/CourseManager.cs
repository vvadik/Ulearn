using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using NUnit.Framework;
using uLearn.CSharp;

namespace uLearn.Web.Models
{
	public class CourseManager
	{
		private static readonly ResourceLoader Resources = new ResourceLoader(typeof (CourseManager));
		public static CourseManager AllCourses = LoadAllCourses();

		public readonly List<Course> Courses = new List<Course>();


		public static CourseManager LoadAllCourses()
		{
			var result = new CourseManager();
			result.AddCourse("Linq", "Linq");
			result.AddCourse("BasicProgramming", "BasicProgramming");
			return result;
		}


		private void AddCourse(string courseId, string courseTitle)
		{
			Courses.Add(LoadCourse(courseId, courseTitle));
		}

		private static Course LoadCourse(string courseId, string courseTitle)
		{
			var slideFiles = Resources.EnumerateResourcesFrom("uLearn.Web.Courses." + courseId)
				.OrderBy(f => f.Filename)
				.ToList();
			var slides = slideFiles.Select(f =>
			{
				var ans = SlideParser.ParseCode(Encoding.UTF8.GetString(f.GetContent()));
				ans.Info = GetInfoForSlide(f, slideFiles);
				return ans;
			})
			.Where(x => x.Info.FileName.Split('.').Last() != "txt")
			.ToArray();
			return new Course(courseId, courseTitle, slides);
		}

		public static LocationSlideInfo GetInfoForSlide(ResourceFile file, IEnumerable<ResourceFile> all)
		{
			var info = new LocationSlideInfo("","","","");
			var detailedPath = file.FullName.Split('.').ToArray();
			if (detailedPath.Last() == "txt")
				return new LocationSlideInfo("res.txt","","","");
			info.FileName = detailedPath[detailedPath.Length - 2] + "." + detailedPath.Last();
			info.BlockName = ExtractBlockOrUnitName(all, detailedPath, ".BlockName.txt"); ;
			info.UnitName = ExtractBlockOrUnitName(all, detailedPath, ".UnitName.txt");
			info.DesiredTitle = ExtractTitle(Encoding.UTF8.GetString(file.GetContent()));
			return info;
		}

		public static string ExtractBlockOrUnitName(IEnumerable<ResourceFile> all, string[] detailedPath, string name)
		{
			var index = name == ".BlockName.txt" ? 3 : 2;
			return Encoding.UTF8.GetString(
				all.First(x => 
					x.FullName == 
					string.Join(".", detailedPath.Take(detailedPath.Length - index)) + name)
								   .GetContent());
			
		}

		public static string ExtractTitle(string content)
		{

			var attribute = new Regex(@"\[(Title.*?)\]");
			var argument = new Regex(@"\((.*?)\)");
			var quad = attribute.Match(content).ToString();
			var ans = argument.Match(quad).ToString();
			if (ans != "")
				return ans.Substring(2,ans.Length-4);
			return "undefined";
		}

		public Course GetCourse(string courseId)
		{
			Course course = Courses.FirstOrDefault(c => c.Id == courseId);
			if (course == null) throw new Exception(string.Format("Course {0} not found", courseId));
			return course;
		}
	}
}