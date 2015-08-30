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
		public override void DoExecute()
		{
			var profile = Config.GetProfile(Profile);
			var credentials = Credentials.GetCredentials(Dir, Profile);

			EdxInteraction.Download(Dir, Config, profile.EdxStudioUrl, credentials);
			
			var edxCourse = EdxCourse.Load(Dir + "/olx");
			if (edxCourse.CourseWithChapters.Chapters.Length != 0)
			{
				Console.WriteLine("List of chapters to be removed or replaced:");
				foreach (var chapterName in edxCourse.CourseWithChapters.Chapters.Select(x => x.DisplayName))
					Console.WriteLine("\t" + chapterName);
				while (true)
				{
					Console.WriteLine("Do you want to proceed? (y/n)");
					var key = Console.ReadKey();
					if (key.Key == ConsoleKey.Y)
						break;
					if (key.Key == ConsoleKey.N)
						return;
				}
			}
			var video = LoadVideoInfo();
			VideoHistory.UpdateHistory(Dir, video);

			Console.WriteLine("Loading uLearn course from {0}", Config.ULearnCourseId);
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(Path.Combine(Dir, Config.ULearnCourseId)));

			Console.WriteLine("Converting uLearn course \"{0}\" to Edx course", course.Id);
			Converter.ToEdxCourse(
				course,
				Config,
				profile.UlearnUrl + ExerciseUrlFormat,
				profile.UlearnUrl + SolutionsUrlFormat,
				video.Records.ToDictionary(x => x.Data.Id, x => Utils.GetNormalizedGuid(x.Guid))
				).Save(Dir + "/olx");

			EdxInteraction.Upload(Dir, course.Id, Config, profile.EdxStudioUrl, credentials);
		}

		private Video LoadVideoInfo()
		{
			var videoFile = string.Format("{0}/{1}", Dir, Config.Video);
			return File.Exists(videoFile)
				? JsonConvert.DeserializeObject<Video>(File.ReadAllText(Config.Video))
				: new Video { Records = new Record[0] };
		}
	}
}