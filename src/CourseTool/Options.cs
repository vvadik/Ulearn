using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn.CourseTool.Json;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.CourseTool
{
	abstract class AbstractOptions
	{
		public bool Start(string dir, string configFile)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			if (!File.Exists(configFile))
			{
				var configTemplateFile = Path.Combine(Utils.GetRootDirectory(), "templates/config.xml");
				File.Copy(configTemplateFile, configFile);
				Process.Start("notepad", configFile);
				Console.WriteLine("Edit the config file {0} and run this option again.", configFile);
				return true;
			}
			return false;
		}

		public abstract int Execute();
	}

	[Verb("convert", HelpText = "Convert uLearn course to Edx course")]
	class ConvertOptions : AbstractOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project")]
		public string Dir { get; set; }

		public override int Execute()
		{
			Dir = Dir ?? Directory.GetCurrentDirectory();
			var configFile = Dir + "/config.xml";

			if (Start(Dir, configFile))
				return 0;

			var config = new FileInfo(configFile).DeserializeXml<Config>();
			var credentials = Credentials.GetCredentials(Dir);

			DownloadManager.Download(Dir, config, credentials);
			
			try
			{
				var edxCourse = EdxCourse.Load(Dir + "/olx");
				if (edxCourse.CourseWithChapters.Chapters.Length != 0)
					Console.WriteLine("List of chapters to be removed or replaced:");
				foreach (var result in edxCourse.CourseWithChapters.Chapters.Select(x => x.DisplayName))
					Console.WriteLine("\t" + result);
				Console.WriteLine("Do you want to proceed?");
				Console.WriteLine("y/n");
				while (true)
				{
					var key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Y)
						break;
					if (key.Key == ConsoleKey.N)
						return 0;
				}
			}
			catch (Exception)
			{
			}

			var videoFile = string.Format("{0}/{1}", Dir, config.Video);
			var video = File.Exists(videoFile) 
				? JsonConvert.DeserializeObject<Video>(File.ReadAllText(config.Video)) 
				: new Video { Records = new Record[0] };

			VideoHistory.UpdateHistory(Dir, video);

			Console.WriteLine("Loading uLearn course from {0}", config.ULearnCourseId);
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", Dir, config.ULearnCourseId)));

			Console.WriteLine("Converting uLearn course \"{0}\" to Edx course", course.Id);
			Converter.ToEdxCourse(
				course,
				config,
				video.Records.ToDictionary(x => x.Data.Id, x => Utils.GetNormalizedGuid(x.Guid))
			).Save(Dir + "/olx");

			DownloadManager.Upload(Dir, course.Id, config, credentials);
			return 0;
		}
	}

	abstract class PatchOptions : AbstractOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project")]
		public string Dir { get; set; }

		[Option('r', "replace", HelpText = "If set, patch replaces Edx slides on uLearn slides with same guid")]
		public bool ReplaceExisting { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }

		public override int Execute()
		{
			Dir = Dir ?? Directory.GetCurrentDirectory();
			var configFile = Dir + "/config.xml";

			if (Start(Dir, configFile))
				return 0;

			var config = new FileInfo(configFile).DeserializeXml<Config>();
			var credentials = Credentials.GetCredentials(Dir);

			DownloadManager.Download(Dir, config, credentials);

			var edxCourse = EdxCourse.Load(Dir + "/olx");

			Patch(new OlxPatcher(Dir + "/olx"), config, edxCourse);

			DownloadManager.Upload(Dir, edxCourse.CourseName, config, credentials);
			return 0;
		}

		public abstract void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse);
	}

	[Verb("patch_ulearn", HelpText = "Patch Edx course with new slides from uLearn course")]
	class ULearnPatchOptions : PatchOptions
	{
		public override void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse)
		{
			var ulearnCourse = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", Dir, config.ULearnCourseId)));

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
					.Where(s => !config.IgnoredSlides.Contains(s.Id))
					.Where(x => guids == null || guids.Contains(x.Guid))
					.Select(x => x.ToVerticals(
							ulearnCourse.Id, 
							config.ExerciseUrl, 
							config.SolutionsUrl, 
							videoGuids,
							config.LtiId
						).ToArray()),
				guids != null || ReplaceExisting
			);
		}
	}

	[Verb("patch_video", HelpText = "Patch Edx course with videos from json file")]
	class VideoPatchOptions : PatchOptions
	{
		public override void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse)
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
					guids != null || ReplaceExisting
				);
			}
		}
	}

	[Verb("patch_custom", HelpText = "Patch Edx course with new slides or videos")]
	class CustomPatchOptions : PatchOptions
	{
		[Option('s', "source", HelpText = "Source directory for custom slides", Required = true)]
		public string SourceDir { get; set; }

		public override void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse)
		{
			// input
			// patcher.PatchComponents();
			// patcher.PatchVerticals();
		}
	}
}
