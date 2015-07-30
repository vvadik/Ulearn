using System.Collections.Generic;
using System.Linq;
using uLearn;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	public static class Converter
	{
		private static Sequential[] CourseToSequentials(Course course, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids, string ltiId)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(
					x => new Sequential(course.Id + "-1-" + x, units[x], 
						course.Slides
							.Where(y => y.Info.UnitName == units[x])
							.SelectMany(y => y.ToVerticals(course.Id, exerciseUrl, solutionsUrl, videoGuids, ltiId))
							.ToArray()
					)
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(
			Course course, string organization, string[] advancedModules, string[] ltiPassports, string exerciseUrl, string solutionsUrl,
			Dictionary<string, string> youtubeId2UlearnVideoIds, string ltiId)
		{
			return new EdxCourse(
				course.Id, organization, course.Title, advancedModules, ltiPassports, 
				new [] { new Chapter(course.Id + "-1", course.Title, CourseToSequentials(course, exerciseUrl, solutionsUrl, youtubeId2UlearnVideoIds, ltiId)) }
			);
		}
	}
}
