using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ITempCoursesRepo
	{
		Task<TempCourse> FindAsync(string courseId);
		Task<List<TempCourse>> GetTempCoursesAsync();
		Task<TempCourseError> GetCourseErrorAsync(string courseId);
		Task<TempCourse> AddTempCourseAsync(string courseId, string authorId);
		Task<DateTime> UpdateTempCourseLoadingTimeAsync(string courseId);
		Task<TempCourseError> UpdateOrAddTempCourseErrorAsync(string courseId, string error);
		Task MarkTempCourseAsNotErroredAsync(string courseId);
	}
}