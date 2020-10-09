using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Core.Courses;

namespace Database.Repos
{
	public interface IUnitsRepo
	{
		Task<List<Guid>> GetVisibleUnitIdsAsync(Course course, string userId);
		Task<List<Guid>> GetPublishedUnitIdsAsync(Course course);
		Task<List<UnitAppearance>> GetUnitAppearancesAsync(Course course);
		Task<bool> IsUnitVisibleForStudents(Course course, Guid unitId);
		DateTime? GetNextUnitPublishTime(string courseId);
		HashSet<string> GetVisibleCourses();
		Task<bool> IsCourseVisibleForStudentsAsync(string courseId);
	}
}