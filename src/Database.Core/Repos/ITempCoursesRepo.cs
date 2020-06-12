using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ITempCoursesRepo
	{
		TempCourse Find(string courseId);
		List<TempCourse> GetTempCourses();
		TempCourseError GetCourseError(string courseId);
		Task<TempCourse> AddTempCourse(string courseId, string authorId);
		Task UpdateTempCourseLoadingTime(string courseId);
		Task UpdateTempCourseLastUpdateTime(string courseId);
		Task<TempCourseError> UpdateOrAddTempCourseError(string courseId, string error);
		Task MarkTempCourseAsNotErrored(string courseId);
	}
}