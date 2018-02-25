using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Database.Extensions;
using Database.Models;
using uLearn;

namespace Database.Repos
{
	public class UnitsRepo
	{
		private readonly UlearnDb db;

		public UnitsRepo(UlearnDb db)
		{
			this.db = db;
		}

		public List<Unit> GetVisibleUnits(Course course, IPrincipal user)
		{
			var canSeeEverything = user.HasAccessFor(course.Id, CourseRole.Tester);
			if (canSeeEverything)
				return course.Units;

			var visibleUnitsIds = new HashSet<Guid>(db.UnitAppearances
				.Where(u => u.CourseId == course.Id && u.PublishTime <= DateTime.Now)
				.Select(u => u.UnitId));
			return course.Units.Where(u => visibleUnitsIds.Contains(u.Id)).ToList();
		}

		public DateTime GetNextUnitPublishTime(string courseId)
		{
			return db.UnitAppearances.Where(u => u.CourseId == courseId && u.PublishTime > DateTime.Now)
				.Select(u => u.PublishTime)
				.Concat(new[] { DateTime.MaxValue }).Min();
		}
	}
}