using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ulearn.Core.Courses;

namespace Database.Repos
{
	public interface IUnitsRepo
	{
		Task<IEnumerable<Guid>> GetVisibleUnitIdsAsync(Course course, string userId);
		IEnumerable<Guid> GetVisibleUnitIds(Course course);
		DateTime? GetNextUnitPublishTime(string courseId);
		HashSet<string> GetVisibleCourses();
	}
}