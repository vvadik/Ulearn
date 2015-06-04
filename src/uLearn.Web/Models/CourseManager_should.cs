using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace uLearn.Web.Models
{
	[TestFixture]
	public class CourseManager_should
	{
		[Test, UseReporter(typeof(DiffReporter))]
		public void load_all_courses()
		{
			var manager = WebCourseManager.Instance;
			var courses = manager.GetCourses().ToList();
			foreach (var c in courses)
			{
				Console.WriteLine("Course: " + c.Id + " " + c.Title);
				foreach (var s in c.Slides)
					Console.WriteLine(s.Info.UnitName + " " + s.Title);
				Console.WriteLine();
			}
			Approvals.VerifyAll(courses.Where(c => !c.Id.StartsWith("BasicProgramming")), "Courses");
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void load_ForTestsCourse_slides()
		{
			var manager = WebCourseManager.Instance;
			Approvals.VerifyAll(manager.GetCourse("ForTests").Slides, "Slides");
			
		}
	}
}
