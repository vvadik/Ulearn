using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	public static class Converter
	{
		private static Sequential[] UnitToSequentials(Course course, Config config, List<string> units, int unitIndex, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids)
		{
			var result = new List<Sequential>
			{
				new Sequential(string.Format("{0}-{1}-{2}", course.Id, unitIndex, 0), units[unitIndex],
					course.Slides
						.Where(s => !config.IgnoredUlearnSlides.Contains(s.Id))
						.Where(y => y.Info.UnitName == units[unitIndex])
						.SelectMany(y => y.ToVerticals(course.Id, exerciseUrl, solutionsUrl, videoGuids, config.LtiId))
						.ToArray())
			};
			var note = course.FindInstructorNote(units[unitIndex]);
			var displayName = "Заметки преподавателю";
			var sequentialId = string.Format("{0}-{1}-{2}", course.Id, unitIndex, "note-seq");
			var verticalId = string.Format("{0}-{1}-{2}", course.Id, unitIndex, "note-vert");
			var mdBlockId = string.Format("{0}-{1}-{2}", course.Id, unitIndex, "note-md");
			if (note != null)
				result.Add(new Sequential(sequentialId, displayName,
					new[]
					{
						new Vertical(verticalId, displayName, new[] { new MdBlock(course.FindInstructorNote(units[unitIndex]).Markdown).ToEdxComponent(mdBlockId, displayName, course.GetDirectoryByUnitName(units[unitIndex])) })
					}) { VisibleToStaffOnly = "true" }
				);
			return result.ToArray();
		}

		private static Chapter[] CourseToChapters(Course course, Config config, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(x => new Chapter(string.Format("{0}-{1}", course.Id, x), units[x], UnitToSequentials(course, config, units, x, exerciseUrl, solutionsUrl, videoGuids)))
				.ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, Config config, string exerciseUrl, string solutionsUrl,
			Dictionary<string, string> youtubeId2UlearnVideoIds)
		{
			return new EdxCourse(course.Id, config.Organization, course.Title, new[] { "lti" }, null, CourseToChapters(course, config, exerciseUrl, solutionsUrl, youtubeId2UlearnVideoIds));
		}
	}
}
