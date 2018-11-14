using System;
using System.Collections.Generic;
using System.Security.Principal;
using uLearn;
using uLearn.Courses;

namespace Database.Repos
{
	public interface IUnitsRepo
	{
		List<Unit> GetVisibleUnits(Course course, IPrincipal user);
		DateTime GetNextUnitPublishTime(string courseId);
	}
}