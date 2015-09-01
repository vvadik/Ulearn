using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn.CourseTool.Json;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	[Verb("patch_ulearn", HelpText = "Patch Edx course with new slides from uLearn course")]
	class ULearnPatchOptions : PatchOptions
	{
		public override void Patch(OlxPatcher patcher, Config config, Profile profile, EdxCourse edxCourse)
		{
			var ulearnDir = new DirectoryInfo(string.Format("{0}/{1}", Dir, config.ULearnCourseId));
			Console.WriteLine("Loading Ulearn course from {0}", ulearnDir.Name);
			var ulearnCourse = new CourseLoader().LoadCourse(ulearnDir);

			var videoJson = string.Format("{0}/{1}", Dir, config.Video);
			var video = File.Exists(videoJson)
				? JsonConvert.DeserializeObject<Video>(File.ReadAllText(videoJson))
				: new Video { Records = new Record[0] };
			var videoHistory = VideoHistory.UpdateHistory(Dir, video);
			var videoGuids = videoHistory.Records
				.SelectMany(x => x.Data.Select(y => Tuple.Create(y.Id, Utils.GetNormalizedGuid(x.Guid))))
				.ToDictionary(x => x.Item1, x => x.Item2);

			var guids = Guids == null ? null : Guids.Split(',').Select(Utils.GetNormalizedGuid).ToList();
			
			patcher.PatchVerticals(
				edxCourse, 
				ulearnCourse.Slides
					.Where(s => !config.IgnoredUlearnSlides.Contains(s.Id))
					.Where(s => guids == null || guids.Contains(s.Guid))
					.Select(s => s.ToVerticals(
						ulearnCourse.Id, 
						profile.UlearnUrl + ExerciseUrlFormat, 
						profile.UlearnUrl + SolutionsUrlFormat, 
						videoGuids,
						config.LtiId
						).ToArray()),
				guids != null || !SkipExistingGuids
				);

			PatchInstructorsNotes(edxCourse, ulearnCourse, patcher.OlxPath);
		}

		private void PatchInstructorsNotes(EdxCourse edxCourse, Course ulearnCourse, string olxPath)
		{
			var ulearnUnits = ulearnCourse.GetUnits().ToList();
			foreach (var chapter in edxCourse.CourseWithChapters.Chapters)
			{
				var chapterNote = ulearnCourse.InstructorNotes.FirstOrDefault(x => x.UnitName == chapter.DisplayName);
				if (chapterNote == null)
					continue;
				var unitIndex = ulearnUnits.IndexOf(chapterNote.UnitName);
				var displayName = "Заметки преподавателю";
				var sequentialId = string.Format("{0}-{1}-{2}", ulearnCourse.Id, unitIndex, "note-seq");
				var verticalId = string.Format("{0}-{1}-{2}", ulearnCourse.Id, unitIndex, "note-vert");
				var mdBlockId = string.Format("{0}-{1}-{2}", ulearnCourse.Id, unitIndex, "note-md");
				var sequentialNote = new Sequential(sequentialId, displayName,
					new[]
					{
						new Vertical(verticalId, displayName, new[] { new MdBlock(chapterNote.Markdown).ToEdxComponent(mdBlockId, displayName, ulearnCourse.GetDirectoryByUnitName(chapterNote.UnitName)) })
					}) { VisibleToStaffOnly = true };
				if (!File.Exists(string.Format("{0}/sequential/{1}.xml", olxPath, sequentialNote.UrlName)))
				{
					var sequentials = chapter.Sequentials.ToList();
					sequentials.Add(sequentialNote);
					new Chapter(chapter.UrlName, chapter.DisplayName, chapter.Start, sequentials.ToArray()).Save(olxPath);
				}
				sequentialNote.Save(olxPath);
			}
		}
	}
}