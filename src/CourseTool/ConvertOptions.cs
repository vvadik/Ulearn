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
		[Option('t', "tar-gz", HelpText = "Filepath of course tar.gz file")]
		public string CourseTarGz{ get; set; }

		public override void DoExecute()
		{
			var profile = Config.GetProfile(Profile);

			Console.WriteLine("Please, download course from Edx (tar.gz from Tools - Export menu) and save it in working directory");


			var tarGzPath = Dir.GetSingleFile(CourseTarGz ?? "*.tar.gz");

			EdxInteraction.ExtractEdxCourseArchive(Dir, tarGzPath);

			Console.WriteLine("Loading edx course...");
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

			Console.WriteLine($"Loading ulearn course from {Config.ULearnCourseId}");
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(Path.Combine(Dir, Config.ULearnCourseId)));

			Console.WriteLine($"Converting ulearn course \"{course.Id}\" to edx course");
			Converter.ToEdxCourse(
				course,
				Config,
				profile.UlearnUrl + SlideUrlFormat,
				profile.UlearnUrl + SolutionsUrlFormat,
				video.Records.ToDictionary(x => x.Data.Id, x => x.Guid.GetNormalizedGuid())
				).Save(Dir + "/olx");

			EdxInteraction.CreateEdxCourseArchive(Dir, course.Id);

			Console.WriteLine($"Now you can upload {course.Id}.tar.gz to edx via Tools - Import menu");
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