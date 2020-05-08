using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Database.Repos.CourseRoles;
using Microsoft.AspNet.Identity;
using Ulearn.Core.Courses;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UnitsRepo : IUnitsRepo
	{
		private readonly UlearnDb db;
		private readonly WebCourseManager courseManager;
		private readonly ICourseRolesRepo courseRolesRepo;

		public UnitsRepo(UlearnDb db, WebCourseManager courseManager, ICourseRolesRepo courseRolesRepo)
		{
			this.db = db;
			this.courseManager = courseManager;
			this.courseRolesRepo = courseRolesRepo;
		}

		public async Task<IEnumerable<Guid>> GetVisibleUnitIdsAsync(Course course, string userId)
		{
			var canSeeEverything = await courseRolesRepo.HasUserAccessToCourseAsync(userId, course.Id, CourseRoleType.Tester);
			if (canSeeEverything)
				return course.GetUnitsNotSafe().Select(u => u.Id);

			return GetVisibleUnitIds(course);
		}

		public IEnumerable<Guid> GetVisibleUnitIds(Course course)
		{
			var visibleUnitsIds = new HashSet<Guid>(db.UnitAppearances
				.Where(u => u.CourseId == course.Id && u.PublishTime <= DateTime.Now)
				.Select(u => u.UnitId));
			return course.GetUnitsNotSafe().Select(u => u.Id).Where(g => visibleUnitsIds.Contains(g));
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
					var units = courseManager.GetCourse(g.Key).GetUnitsNotSafe().Select(u => u.Id).ToHashSet();
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