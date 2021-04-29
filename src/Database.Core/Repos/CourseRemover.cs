using System.Threading.Tasks;

namespace Database.Repos
{
	public interface ICourseRemover
	{
		Task RemoveCourseWithAllData(string courseId);
	}

	public class CourseRemover: ICourseRemover
	{
		public async Task RemoveCourseWithAllData(string courseId)
		{
			
		}
	}
}