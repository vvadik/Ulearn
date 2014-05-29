using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var slides = slideFiles.Select(f => SlideParser.ParseCode(Encoding.UTF8.GetString(f.GetContent()))).ToArray();
			return new Course(courseId, courseTitle, slides);
		}

		public Course GetCourse(string courseId)
		{
			Course course = courses.FirstOrDefault(c => c.Id == courseId);
			if (course == null) throw new Exception(string.Format("Course {0} not found", courseId));
			return course;
		}
	}
}