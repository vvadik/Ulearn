using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ulearn.Core.Courses;

namespace Database.Repos
{
	public interface IUnitsRepo
	{
		Task<List<Guid>> GetVisibleUnitIdsAsync(Course course, string userId);
		Task<List<Guid>> GetPublishedUnitIdsAsync(Course course);
		DateTime? GetNextUnitPublishTime(string courseId);
		HashSet<string> GetVisibleCourses();
		Task<bool> IsCourseVisibleForStudentsAsync(string courseId);
	}
}