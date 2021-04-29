using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ITempCoursesRepo
	{
		Task<TempCourse> Find(string courseId);
		Task<List<TempCourse>> GetTempCourses();
		Task<TempCourseError> GetCourseError(string courseId);
		Task<TempCourse> AddTempCourse(string courseId, string authorId);
		Task<DateTime> UpdateTempCourseLoadingTime(string courseId);
		Task<DateTime> UpdateTempCourseLastUpdateTime(string courseId);
		Task<TempCourseError> UpdateOrAddTempCourseError(string courseId, string error);
		Task MarkTempCourseAsNotErrored(string courseId);
		Task RemoveTempCourse(string baseCourseId, string authorId, IServiceProvider serviceProvider);
	}
}