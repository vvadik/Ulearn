using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	public static class Converter
	{
		private static Sequential[] UnitToSequentials(Course course, Config config, List<Unit> units, int unitIndex, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids)
		{
			var unit = units[unitIndex];
			var result = new List<Sequential>
			{
				new Sequential($"{course.Id}-{unitIndex}-{0}", unit.Title,
					unit.Slides
						.Where(s => !config.IgnoredUlearnSlides.Select(Guid.Parse).Contains(s.Id))
						.SelectMany(y => y.ToVerticals(course.Id, exerciseUrl, solutionsUrl, videoGuids, config.LtiId))
						.ToArray())
			};
			var note = unit.InstructorNote;
			if (note != null && config.EmitSequentialsForInstructorNotes)
			{
				var displayName = "Заметки преподавателю";
				var sequentialId = $"{course.Id}-{unitIndex}-note-seq";
				var verticalId = $"{course.Id}-{unitIndex}-note-vert";
				var mdBlockId = $"{course.Id}-{unitIndex}-note-md";
				result.Add(new Sequential(sequentialId, displayName,
						new[]
						{
							new Vertical(
								verticalId,
								displayName,
								new[]
								{
									new MdBlock(unit.InstructorNote.Markdown)
										.ToEdxComponent(mdBlockId, displayName, unit.Directory.FullName)
								})
						}) { VisibleToStaffOnly = true }
				);
			}
			return result.ToArray();
		}

		private static Chapter[] CourseToChapters(Course course, Config config, string exerciseUrl, string solutionsUrl, Dictionary<string, string> videoGuids)
		{
			var units = course.Units;
			return Enumerable
				.Range(0, units.Count)
				.Select(idx => new Chapter(
					$"{course.Id}-{idx}",
					units[idx].Title,
					null,
					UnitToSequentials(course, config, units, idx, exerciseUrl, solutionsUrl, videoGuids)))
				.ToArray();
		}

		public static EdxCourse ToEdxCourse(Course course, Config config, string exerciseUrl, string solutionsUrl,
			Dictionary<string, string> youtubeId2UlearnVideoIds)
		{
			return new EdxCourse(
				course.Id,
				config.Organization,
				course.Title,
				new[] { "lti" },
				null,
				CourseToChapters(course, config, exerciseUrl, solutionsUrl, youtubeId2UlearnVideoIds));
		}
	}
}