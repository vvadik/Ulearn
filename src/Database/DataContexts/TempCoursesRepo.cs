using System.Collections.Generic;
using System.Linq;
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
	}
}