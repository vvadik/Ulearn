using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	public static class Converter
	{
		private static Chapter[] CourseToChapters(Course course, Config config, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(x => new Chapter(string.Format("{0}-{1}", course.Id, x), units[x], new[] {
						new Sequential(string.Format("{0}-{1}-{2}", course.Id, x, 0), units[x], 
							course.Slides
								.Where(s => !config.IgnoredUlearnSlides.Contains(s.Id))
								.Where(y => y.Info.UnitName == units[x])
								.SelectMany(y => y.ToVerticals(course.Id, exerciseUrl, solutionsUrl, videoGuids, config.LtiId))
								.ToArray()
						)
					})
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, Config config, string exerciseUrl, string solutionsUrl,
			Dictionary<string, string> youtubeId2UlearnVideoIds)
		{
			return new EdxCourse(course.Id, config.Organization, course.Title, new[] { "lti" }, null, CourseToChapters(course, config, exerciseUrl, solutionsUrl, youtubeId2UlearnVideoIds));
		}
	}
}
