using System;
using System.Collections.Generic;
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

		public List<TempCourse> GetTempCourses()
		{
			return db.TempCourses.ToList();
		}

		public TempCourseError GetCourseError(string courseId)
		{
			return db.TempCourseErrors.SingleOrDefault(error => error.CourseId == courseId);
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
		}

		public async Task UpdateTempCourseLastUpdateTime(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;

			course.LastUpdateTime = DateTime.Now;
			await db.SaveChangesAsync();
		}

		public async Task<TempCourseError> UpdateOrAddTempCourseError(string courseId, string error)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return null;
			var existingError = db.TempCourseErrors.Find(courseId);
			TempCourseError result;
			if (existingError == null)
			{
				var errorEntity = new TempCourseError() { CourseId = courseId, Error = error };
				db.TempCourseErrors.Add(errorEntity);
				result = errorEntity;
			}
			else
			{
				existingError.Error = error;
				result = existingError;
			}

			await db.SaveChangesAsync();
			return result;
		}

		public async Task MarkTempCourseAsNotErrored(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;
			var error = db.TempCourseErrors.Find(courseId);
			if (error == null)
			{
				await UpdateOrAddTempCourseError(courseId, null);
				return;
			}

			error.Error = null;
			await db.SaveChangesAsync();
		}
	}
}