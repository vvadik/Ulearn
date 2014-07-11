using System;
using NUnit.Framework;

namespace uLearn.Web.Models
{
	[TestFixture]
	public class CourseManager_should
	{
		[Test]
		public void load_all_courses()
		{
			var manager = CourseManager.LoadAllCourses();
			foreach (var c in manager.Courses)
			{
				foreach (var s in c.Slides)
				{
					Console.WriteLine("FileName: " + s.Info.FileName);
					Console.WriteLine("BlockName: " + s.Info.CourseName);
					Console.WriteLine("UnitName: " + s.Info.UnitName);
					Console.WriteLine("Title: " + s.Title);
					Console.WriteLine("*********************");
				}
			}
			Assert.That(manager.Courses.Count, Is.EqualTo(2));
			Assert.That(manager.GetCourse("Linq").Slides.Length, Is.GreaterThan(10));
		}
	}
}
