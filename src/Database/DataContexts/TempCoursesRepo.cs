using System;
using System.Collections.Generic;
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
			return db.TempCourses.Find(courseId);
		}

		public List<TempCourse> GetTempCourses()
		{
			return db.TempCourses.ToList();
		}

		public List<TempCourse> GetTempCoursesNoTracking()
		{
			return db.TempCourses.AsNoTracking().ToList();
		}

		public void UpdateTempCourseLastUpdateTime(string courseId)
		{
			var course = db.TempCourses.Find(courseId);
			if (course == null)
				return;

			course.LastUpdateTime = DateTime.Now;
			db.SaveChanges();
		}
	}
}