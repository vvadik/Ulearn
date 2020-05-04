using System;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.DataContexts
{
	public class TempCoursesRepo
	{
		private readonly ULearnDb db;

		public TempCoursesRepo()
			: this(new ULearnDb())
		{
		}

		public TempCoursesRepo(ULearnDb db)
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
				LoadingTime = DateTime.Now
			};
			var result = db.TempCourses.Add(tempCourse);
			await db.SaveChangesAsync();
			return result;
		}

		public async Task UpdateTempCourseLoadingTime(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;

			course.LoadingTime = DateTime.Now;
			await db.SaveChangesAsync();
		}
	}
}