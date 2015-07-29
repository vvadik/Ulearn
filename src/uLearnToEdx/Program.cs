using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using uLearn;
using uLearn.Model.Edx;
using uLearnToEdx.Json;

namespace uLearnToEdx
{
	public class Program
	{
		private static void Start(StartOptions options)
		{
			File.WriteAllText("config.xml", new Config
			{
				WorkingDir = options.Dir,
				AdvancedModules = options.AdvancedModules.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries),
				CourseNumber = options.CourseNumber,
				CourseRun = options.CourseRun,
				Hostname = options.Hostname,
				LtiPassports = options.LtiPassports.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries),
				Organization = options.Organization,
				Port = options.Port,
				ExerciseUrl = options.ExerciseUrl,
				SolutionsUrl = options.SolutionsUrl,
			}.XmlSerialize());
			Console.WriteLine("Enter email:");
			var email = Console.ReadLine();
			Console.WriteLine("Enter password:");
			var password = Utils.GetPass();

			File.WriteAllText("credentials.xml", new Credentials(email, password).XmlSerialize());

			Directory.Delete(options.Dir, true);
			Directory.CreateDirectory(options.Dir);

			DownloadManager.Download(options.Hostname, options.Port, email, password, options.Organization, options.CourseNumber, options.CourseRun, options.CourseRun + ".tar.gz");
			ArchiveManager.ExtractTar(options.CourseRun + ".tar.gz", options.Dir);

			Directory.Move(string.Format("{0}/{1}", options.Dir, options.CourseRun), string.Format("{0}/olx", options.Dir));
		}

		private static void Convert(ConvertOptions options)
		{
			var config = new FileInfo("config.xml").DeserializeXml<Config>();
			Converter.ToEdxCourse(
				new CourseLoader().LoadCourse(new DirectoryInfo(options.InputDir)), 
				config.Organization, 
				config.AdvancedModules, 
				config.LtiPassports, 
				config.ExerciseUrl, 
				config.SolutionsUrl,
				options.VideoJson != null && File.Exists(options.VideoJson)
					? JsonConvert.DeserializeObject<Video>(File.ReadAllText(options.VideoJson)).Records
						.ToDictionary(x => x.Data.Id, x => x.Guid.ToString("D"))
					: new Dictionary<string, string>()
			).Save(config.WorkingDir + "/olx");
		}

		private static void Patch(PatchOptions options)
		{
			var config = new FileInfo("config.xml").DeserializeXml<Config>();
			var credentials = new FileInfo("credentials.xml").DeserializeXml<Credentials>();
			var edxCourse = EdxCourse.Load(config.WorkingDir + "/olx");
			
			if (options.InputDir != null)
			{
				var ulearnCourse = new CourseLoader().LoadCourse(new DirectoryInfo(options.InputDir));
				
				Console.WriteLine("Patching {0} with slides from {1}...", config.CourseRun, ulearnCourse.Id);
				edxCourse.PatchSlides(config.WorkingDir + "/olx", ulearnCourse.Id, ulearnCourse.Slides, config.ExerciseUrl, config.SolutionsUrl);
			}

			if (options.VideoJson != null && File.Exists(options.VideoJson))
			{
				var video = JsonConvert.DeserializeObject<Video>(File.ReadAllText(options.VideoJson));
				edxCourse.PatchVideos(config.WorkingDir + "/olx", video.Records.ToDictionary(x => x.Guid.ToString("D"), x => Tuple.Create(x.Data.Id, x.Data.Name)));
			}

			Environment.CurrentDirectory = config.WorkingDir;
			Console.WriteLine("Creating {0}.tar.gz...", edxCourse.CourseName);
			Utils.DirectoryCopy("olx", edxCourse.CourseName, true);
			ArchiveManager.CreateTar(edxCourse.CourseName + ".tar.gz", edxCourse.CourseName);

			Console.WriteLine("Uploading {0}.tar.gz...", edxCourse.CourseName);
			DownloadManager.Upload(config.Hostname, config.Port, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, edxCourse.CourseName + ".tar.gz");
			Directory.Delete(edxCourse.CourseName, true);
			File.Delete(edxCourse.CourseName + ".tar.gz");
		}

		public static void Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<StartOptions, ConvertOptions, PatchOptions>(args);
			result.Return(
				(StartOptions options) => { 
					Start(options);
					return 0;
				},
				(ConvertOptions options) => { 
					Convert(options);
					return 0;
				},
				(PatchOptions options) => { 
					Patch(options);
					return 0;
				},
				_ => -1
			);
		}
	}
}
