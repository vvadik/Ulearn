using System;
using System.Threading.Tasks;
using Database.Models;
using System.Linq;

namespace Database.Repos
{
	public class TempCoursesRepo : ITempCoursesRepo
	{
		private readonly UlearnDb db;


		public TempCoursesRepo(UlearnDb db)
		{
			this.db = db;
		}

		public TempCourse Find(string courseId)
		{
			return db.TempCourses.SingleOrDefault(course => course.CourseId == courseId);
		}

		public async Task<TempCourse> AddTempCourse(string courseId, string authorId)
		{
			var tempCourse = new TempCourse()
			{
				CourseId = courseId,
				AuthorId = authorId,
				LoadingTime = DateTime.Now,
				LastUpdateTime = DateTime.Now
			};
			db.TempCourses.Add(tempCourse);
			await db.SaveChangesAsync();
			return tempCourse;
		}

		public async Task UpdateTempCourseLoadingTime(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;

			course.LoadingTime = DateTime.Now;
			await db.SaveChangesAsync();
		}public async Task UpdateTempCourseLastUpdateTime(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;

			course.LastUpdateTime = DateTime.Now;
			await db.SaveChangesAsync();
		}
		
	}
}