using System.Collections.Generic;
using System.Linq;
using uLearn;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	public static class Converter
	{
		private static Chapter[] CourseToChapters(Course course, Config config, Dictionary<string, string> videoGuids)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(x => new Chapter(string.Format("{0}-{1}", course.Id, x), units[x], new [] {
						new Sequential(string.Format("{0}-{1}-{2}", course.Id, x, 0), units[x], 
							course.Slides
								.Where(s => !config.IgnoredSlides.Contains(s.Id))
								.Where(y => y.Info.UnitName == units[x])
								.SelectMany(y => y.ToVerticals(course.Id, config.ExerciseUrl, config.SolutionsUrl, videoGuids, config.LtiId))
								.ToArray()
						)
					})
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, Config config,
			Dictionary<string, string> youtubeId2UlearnVideoIds)
		{
			return new EdxCourse(course.Id, config.Organization, course.Title, null, null, CourseToChapters(course, config, youtubeId2UlearnVideoIds));
		}
	}
}
