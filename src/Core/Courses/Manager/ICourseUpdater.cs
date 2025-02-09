﻿using System.Threading.Tasks;

namespace Ulearn.Core.Courses.Manager
{
	public interface ICourseUpdater
	{
		// Эти же методы загружают курсы в начале работы
		Task UpdateCourses();
		Task UpdateTempCourses();
	}
}