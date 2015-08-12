using System;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;

namespace uLearn.Web.Models
{
	[TestFixture]
	public class WebCourseManager_should
	{
		[Test]
		public void load_all_courses()
		{
			var manager = WebCourseManager.Instance;
			var courses = manager.GetCourses().ToList();
//			foreach (var c in courses)
//				PrintCourse(c);
			Assert.IsTrue(courses.Any(c => c.Id == "ForTests"));
		}

		private static void PrintCourse(Course c)
		{
			Console.WriteLine("Course: " + c.Id + " " + c.Title);
			foreach (var s in c.Slides)
				Console.WriteLine(s.Info.UnitName + " " + s.Title);
			Console.WriteLine();
		}

		[Test]
		public void load_ForTestsCourse_slides()
		{
			var manager = WebCourseManager.Instance;
			Approvals.VerifyAll(manager.GetCourse("ForTests").Slides, "Slides");
		}
	}
}