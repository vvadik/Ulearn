using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Ulearn.Core.Courses
{
	public interface ICoursesStorage
	{
		Course GetCourse(string courseId);
		Course FindCourse(string courseId);
		bool HasCourse(string courseId);

		void AddOrUpdateCourse(Course course, Guid version);
		void TryRemoveCourse(string courseId);
	}

	public class CoursesStorage : ICoursesStorage
	{
		private readonly ConcurrentDictionary<string, Course> courses = new ConcurrentDictionary<string, Course>(StringComparer.InvariantCultureIgnoreCase);

		public Course GetCourse(string courseId)
		{
			if (courses.TryGetValue(courseId, out var course))
				return course;
			throw new CourseNotFoundException(courseId);
		}

		[CanBeNull]
		public Course FindCourse(string courseId)
		{
			if (courses.TryGetValue(courseId, out var course))
				return course;
			return null;
		}

		public bool HasCourse(string courseId)
		{
			return FindCourse(courseId) != null;
		}

		public void AddOrUpdateCourse(Course course, Guid version)
		{
			course.CourseVersion = version;
			courses.AddOrUpdate(course.Id, _ => course, (_, _) => course);
		}

		public void TryRemoveCourse(string courseId)
		{
			courses.TryRemove(courseId, out _);
		}
	}
}