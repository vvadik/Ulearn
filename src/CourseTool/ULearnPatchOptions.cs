using System;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn.CourseTool.Json;
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
				.SelectMany(x => x.Data.Select(y => Tuple.Create<string, string>(y.Id, Utils.GetNormalizedGuid((Guid)x.Guid))))
				.ToDictionary(x => x.Item1, x => x.Item2);

			var guids = Guids == null ? null : Guids.Split(',').Select(Utils.GetNormalizedGuid).ToList();

			patcher.PatchVerticals(
				edxCourse, 
				ulearnCourse.Slides
					.Where(s => !config.IgnoredUlearnSlides.Contains(s.Id))
					.Where(x => guids == null || guids.Contains(x.Guid))
					.Select(x => x.ToVerticals(
						ulearnCourse.Id, 
						profile.UlearnUrl + ExerciseUrlFormat, 
						profile.UlearnUrl + SolutionsUrlFormat, 
						videoGuids,
						config.LtiId
						).ToArray()),
				guids != null || !SkipExistingGuids
				);
		}
	}
}