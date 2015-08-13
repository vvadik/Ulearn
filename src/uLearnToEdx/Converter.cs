using System.Collections.Generic;
using System.Linq;
using uLearn;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	public static class Converter
	{
		private static Chapter[] CourseToChapters(Course course, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids, string ltiId)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(x => new Chapter(string.Format("{0}-{1}", course.Id, x), units[x], new [] {
						new Sequential(string.Format("{0}-{1}-{2}", course.Id, x, 0), units[x], 
							course.Slides
								.Where(y => y.Info.UnitName == units[x])
								.SelectMany(y => y.ToVerticals(course.Id, exerciseUrl, solutionsUrl, videoGuids, ltiId))
								.ToArray()
						)
					})
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, string organization, string exerciseUrl, string solutionsUrl,
			Dictionary<string, string> youtubeId2UlearnVideoIds, string ltiId)
		{
			return new EdxCourse(course.Id, organization, course.Title, null, null, CourseToChapters(course, exerciseUrl, solutionsUrl, youtubeId2UlearnVideoIds, ltiId));
		}
	}
}
