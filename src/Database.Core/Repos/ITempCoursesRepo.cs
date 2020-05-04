using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ITempCoursesRepo
	{
		TempCourse Find(string courseId);


		Task<TempCourse> AddTempCourse(string courseId, string authorId);


		Task UpdateTempCourseLoadingTime(string courseId);
	}
}