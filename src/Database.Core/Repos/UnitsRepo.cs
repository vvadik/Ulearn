using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Database.Repos.CourseRoles;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Ulearn.Core.Courses;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UnitsRepo : IUnitsRepo
	{
		private readonly UlearnDb db;
		private readonly IWebCourseManager courseManager;
		private readonly ICourseRolesRepo courseRolesRepo;

		public UnitsRepo(UlearnDb db, IWebCourseManager courseManager, ICourseRolesRepo courseRolesRepo)
		{
			this.db = db;
			this.courseManager = courseManager;
			this.courseRolesRepo = courseRolesRepo;
		}

		public async Task<List<Guid>> GetVisibleUnitIdsAsync(Course course, string userId)
		{
			var canSeeEverything = await courseRolesRepo.HasUserAccessToCourseAsync(userId, course.Id, CourseRoleType.Tester);
			if (canSeeEverything)
				return course.GetUnitsNotSafe().Select(u => u.Id).ToList();

			return await GetPublishedUnitIdsAsync(course);
		}

		public async Task<List<Guid>> GetPublishedUnitIdsAsync(Course course)
		{
			var visibleUnitsIds = new HashSet<Guid>(await db.UnitAppearances
				.Where(u => u.CourseId == course.Id && u.PublishTime <= DateTime.Now)
				.Select(u => u.UnitId)
				.ToListAsync());
			return course.GetUnitsNotSafe().Select(u => u.Id).Where(g => visibleUnitsIds.Contains(g)).ToList();
		}
		
		public async Task<List<UnitAppearance>> GetUnitAppearancesAsync(Course course)
		{
			return await db.UnitAppearances
				.Where(u => u.CourseId == course.Id)
				.ToListAsync();
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

		public async Task<bool> IsCourseVisibleForStudentsAsync(string courseId)
		{
			if (await courseManager.FindCourseAsync(courseId) == null)
				return false;
			var visibleUnitsIds = await db.UnitAppearances
				.Where(u => u.CourseId == courseId)
				.Where(u => u.PublishTime <= DateTime.Now)
				.Select(u => u.UnitId)
				.ToListAsync();
			var units = (await courseManager.GetCourseAsync(courseId)).GetUnitsNotSafe().Select(u => u.Id).ToHashSet();
			units.IntersectWith(visibleUnitsIds);
			return units.Any();
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