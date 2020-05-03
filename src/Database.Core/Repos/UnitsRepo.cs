using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Database.Extensions;
using Database.Models;
using uLearn;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Units;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UnitsRepo : IUnitsRepo
	{
		private readonly UlearnDb db;
		private readonly WebCourseManager courseManager;

		public UnitsRepo(UlearnDb db, WebCourseManager courseManager)
		{
			this.db = db;
			this.courseManager = courseManager;
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
			var appearances = db.UnitAppearances
				.Where(u => u.PublishTime <= DateTime.Now)
				.Select(u => new { u.CourseId, u.UnitId })
				.AsEnumerable()
				.GroupBy(p => p.CourseId)
				.Where(g => courseManager.FindCourse(g.Key) != null)
				.Where(g =>
				{
					var units = courseManager.GetCourse(g.Key).Units.Select(u => u.Id).ToHashSet();
					units.IntersectWith(g.Select(p => p.UnitId));
					return units.Any();
				})
				.Select(g => g.Key);
			return new HashSet<string>(appearances, StringComparer.OrdinalIgnoreCase);
		}

		public DateTime? GetNextUnitPublishTime(string courseId)
		{
			return db.UnitAppearances
				.Where(u => u.CourseId == courseId && u.PublishTime > DateTime.Now)
				.Select(u => (DateTime?)u.PublishTime)
				.OrderBy(t => t)
				.FirstOrDefault();
		}
	}
}