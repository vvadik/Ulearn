using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;
using uLearnToEdx.Json;

namespace uLearnToEdx
{
	interface IOptions
	{
		int Execute();
	}

	[Verb("start", HelpText = "Perform initial setup for Edx course.")]
	internal class StartOptions : IOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }

		public int Execute()
		{
			Utils.DeleteDirectoryIfExists(Dir);
			Directory.CreateDirectory(Dir);

			File.Copy(string.Format("{0}/templates/config.xml", Utils.GetRootDirectory()), Dir + "/config.xml");
			Credentials.GetCredentials(Dir);

			Process.Start("notepad", Dir + "/config.xml");
			return 0;
		}
	}

	[Verb("convert", HelpText = "Convert uLearn course to Edx course.")]
	class ConvertOptions : IOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }

		[Option('i', "input", HelpText = "Directory with uLearn course to be converted", Required = true)]
		public string InputDir { get; set; }

		[Option('v', "video", HelpText = "Json file with information about video used in the course")]
		public string VideoJson { get; set; }

		public int Execute()
		{
			var config = new FileInfo(Dir + "/config.xml").DeserializeXml<Config>();
			var credentials = Credentials.GetCredentials(Dir);

			Utils.DeleteDirectoryIfExists(string.Format("{0}/{1}", Dir, config.ULearnCourseId));
			Utils.DirectoryCopy(InputDir, string.Format("{0}/{1}", Dir, config.ULearnCourseId), true);
			if (VideoJson != null)
				File.Copy(VideoJson, string.Format("{0}/{1}", Dir, config.Video));

			Console.WriteLine("Loading uLearn course from {0}", config.ULearnCourseId);
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", Dir, config.ULearnCourseId)));

			Console.WriteLine("Converting uLearn course \"{0}\" to Edx course", course.Id);
			Converter.ToEdxCourse(
				course,
				config.Organization,
				config.ExerciseUrl,
				config.SolutionsUrl,
				VideoJson != null && File.Exists(VideoJson)
					? JsonConvert.DeserializeObject<Video>(File.ReadAllText(VideoJson)).Records
						.ToDictionary(x => x.Data.Id, x => Utils.GetNormalizedGuid(x.Guid))
					: new Dictionary<string, string>(),
				config.LtiId
			).Save(Dir + "/olx");

			DownloadManager.Upload(Dir, course.Id, config, credentials);
			return 0;
		}
	}

	abstract class PatchOptions : IOptions
	{
		[Option('d', "dir", HelpText = "Working directory for the project", Required = true)]
		public string Dir { get; set; }

		[Option('r', "replace", HelpText = "If set, patch replaces Edx slides on uLearn slides with same guid")]
		public bool ReplaceExisting { get; set; }

		[Option('g', "guid", HelpText = "Specific guids to be patched separated by comma")]
		public string Guids { get; set; }

		public int Execute()
		{
			var config = new FileInfo(Dir + "/config.xml").DeserializeXml<Config>();
			var credentials = Credentials.GetCredentials(Dir);

			DownloadManager.Download(Dir, config, credentials);

			var edxCourse = EdxCourse.Load(Dir + "/olx");

			Patch(new OlxPatcher(Dir + "/olx"), config, edxCourse);

			DownloadManager.Upload(Dir, edxCourse.CourseName, config, credentials);
			return 0;
		}

		public abstract void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse);
	}

	[Verb("patch", HelpText = "Patch Edx course with new slides or videos.")]
	class ULearnPatchOptions : PatchOptions
	{
		public override void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse)
		{
			var ulearnCourse = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", Dir, config.ULearnCourseId)));

			var videoJson = string.Format("{0}/{1}", Dir, config.Video);
			var videoGuids = File.Exists(videoJson) 
				? JsonConvert.DeserializeObject<Video>(File.ReadAllText(videoJson)).Records.ToDictionary(x => x.Data.Id, x => Utils.GetNormalizedGuid(x.Guid)) 
				: new Dictionary<string, string>();

			patcher.PatchVerticals(
				edxCourse, 
				ulearnCourse.Slides
					.Select(x => x.ToVerticals(
							ulearnCourse.Id, 
							config.ExerciseUrl, 
							config.SolutionsUrl, 
							videoGuids,
							config.LtiId
						).ToArray())
			);

			if (config.Video != null && File.Exists(string.Format("{0}/{1}", Dir, config.Video)))
			{
				var video = JsonConvert.DeserializeObject<Video>(File.ReadAllText(string.Format("{0}/{1}", Dir, config.Video)));
				var videoComponents = video
					.Records
					.ToDictionary(x => Utils.GetNormalizedGuid(x.Guid), x => Tuple.Create(x.Data.Id, x.Data.Name))
					.Select(x => new VideoComponent(x.Key, x.Value.Item2, x.Value.Item1));
				
				patcher.PatchComponents(
					edxCourse, 
					videoComponents
				);
			}
		}
	}

	[Verb("custom", HelpText = "Patch Edx course with new slides or videos")]
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
