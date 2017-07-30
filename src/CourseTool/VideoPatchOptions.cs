using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn.CourseTool.Json;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.CourseTool
{
	[Verb("patch-video", HelpText = "Patch Edx course with videos from json file")]
	class VideoPatchOptions : PatchOptions
	{
		public override void Patch(OlxPatcher patcher, Config config, Profile profile, EdxCourse edxCourse)
		{
			var videoJson = string.Format("{0}/{1}", Dir, config.Video);
			var video = File.Exists(videoJson)
				? JsonConvert.DeserializeObject<Video>(File.ReadAllText(videoJson))
				: new Video { Records = new Record[0] };
			VideoHistory.UpdateHistory(Dir, video);
			var guids = Guids == null ? null : Guids.Split(',').Select(Utils.GetNormalizedGuid).ToList();
			if (config.Video != null && File.Exists(string.Format("{0}/{1}", Dir, config.Video)))
			{
				var videoComponents = video
					.Records
					.Where(x => guids == null || guids.Contains(Utils.GetNormalizedGuid(x.Guid)))
					.Select(x => new VideoComponent(Utils.GetNormalizedGuid(x.Guid), x.Data.Name, x.Data.Id));

				patcher.PatchComponents(
					edxCourse,
					videoComponents,
					guids != null || !SkipExistingGuids
				);
			}
		}
	}
}