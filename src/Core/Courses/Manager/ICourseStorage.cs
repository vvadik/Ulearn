using System.Collections.Generic;

namespace Ulearn.Core.Courses.Manager
{
	public delegate void CourseChangedEventHandler(string courseId);

	public interface ICourseStorage
	{
		Course GetCourse(string courseId);
		IEnumerable<Course> GetCourses();
		Course FindCourse(string courseId);
		bool HasCourse(string courseId);
		event CourseChangedEventHandler CourseChangedEvent;
	}
}