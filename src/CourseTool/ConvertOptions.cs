using System;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn.CourseTool.Json;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
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
}