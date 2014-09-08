using System;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Web.Models
{
	[TestFixture]
	public class CourseManager_should
	{
		[Test]
		public void load_all_courses()
		{
			var manager = WebCourseManager.Instance;
			foreach (var c in manager.GetCourses())
			{
				Console.WriteLine("Course: " + c.Id + " " + c.Title);
				foreach (var s in c.Slides)
					Console.WriteLine(s.Info.UnitName + " " + s.Title);
				Console.WriteLine();
			}
			Assert.That(manager.GetCourses().Count(), Is.EqualTo(3));
			Assert.That(manager.GetCourse("Linq").Slides.Length, Is.GreaterThan(10));
		}
	}
}
