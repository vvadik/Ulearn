using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Database.Extensions;
using Database.Models;
using uLearn;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Units;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UnitsRepo : IUnitsRepo
	{
		private readonly UlearnDb db;

		public UnitsRepo(UlearnDb db)
		{
			this.db = db;
		}

		public List<Unit> GetVisibleUnits(Course course, IPrincipal user)
		{
			var canSeeEverything = user.HasAccessFor(course.Id, CourseRoleType.Tester);
			if (canSeeEverything)
				return course.Units;

			return GetVisibleUnits(course);
		}
		
		public List<Unit> GetVisibleUnits(Course course)
		{
			var visibleUnitsIds = new HashSet<Guid>(db.UnitAppearances
				.Where(u => u.CourseId == course.Id && u.PublishTime <= DateTime.Now)
				.Select(u => u.UnitId));
			return course.Units.Where(u => visibleUnitsIds.Contains(u.Id)).ToList();
		}

		public HashSet<string> GetVisibleCourses()
		{
			return new HashSet<string>(db.UnitAppearances
				.Where(u => u.PublishTime <= DateTime.Now)
				.Select(u => u.CourseId), StringComparer.OrdinalIgnoreCase);
		}

		public DateTime? GetNextUnitPublishTime(string courseId)
		{
			return db.UnitAppearances.Where(u => u.CourseId == courseId && u.PublishTime > DateTime.Now)
				.Select(u => (DateTime?)u.PublishTime).OrderBy(t => t).FirstOrDefault();
		}
	}
}