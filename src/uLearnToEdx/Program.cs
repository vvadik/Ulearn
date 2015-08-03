using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private static Credentials GetCredentials(string dir)
		{
			Credentials credentials;
			if (File.Exists(dir + "/credentials.xml"))
				credentials = new FileInfo(dir + "/credentials.xml").DeserializeXml<Credentials>();
			else
			{
				Console.WriteLine("Enter email:");
				var email = Console.ReadLine();
				Console.WriteLine("Enter password:");
				var password = Utils.GetPass();
				credentials = new Credentials(email, password);
				File.WriteAllText(dir + "/credentials.xml", credentials.XmlSerialize());
			}
			return credentials;
		}

		private static void Start(StartOptions options)
		{
			if (Directory.Exists(options.Dir))
				Directory.Delete(options.Dir, true);
			Directory.CreateDirectory(options.Dir);
			
			File.Copy(string.Format("{0}/templates/config.xml", Utils.GetRootDirectory()), options.Dir + "/config.xml");
			GetCredentials(options.Dir);
			
			Process.Start("notepad", options.Dir + "/config.xml");
		}

		private static void Convert(ConvertOptions options)
		{
			var config = new FileInfo(options.Dir + "/config.xml").DeserializeXml<Config>();
			var credentials = GetCredentials(options.Dir);
			
			if (Directory.Exists(string.Format("{0}/{1}", options.Dir, config.ULearnCourseId)))
				Directory.Delete(string.Format("{0}/{1}", options.Dir, config.ULearnCourseId), true);
			Utils.DirectoryCopy(options.InputDir, string.Format("{0}/{1}", options.Dir, config.ULearnCourseId), true);
			if (options.VideoJson != null)
				File.Copy(options.VideoJson, string.Format("{0}/{1}", options.Dir, config.Video));

			Console.WriteLine("Loading uLearn course from {0}", config.ULearnCourseId);
			var course = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", options.Dir, config.ULearnCourseId)));

			Console.WriteLine("Converting uLearn course \"{0}\" to Edx course", course.Id);
			Converter.ToEdxCourse(
				course, 
				config.Organization, 
				config.ExerciseUrl, 
				config.SolutionsUrl,
				options.VideoJson != null && File.Exists(options.VideoJson)
					? JsonConvert.DeserializeObject<Video>(File.ReadAllText(options.VideoJson)).Records
						.ToDictionary(x => x.Data.Id, x => x.Guid.ToString("D"))
					: new Dictionary<string, string>(),
				config.LtiId
			).Save(options.Dir + "/olx");

			Upload(options.Dir, course.Id, config, credentials);
		}

		private static void Patch(PatchOptions options)
		{
			var config = new FileInfo(options.Dir + "/config.xml").DeserializeXml<Config>();
			var credentials = GetCredentials(options.Dir);

			Console.WriteLine("Downloading {0}.tar.gz", config.CourseRun);
			DownloadManager.Download(config.Hostname, config.Port, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, config.CourseRun + ".tar.gz");
			
			ArchiveManager.ExtractTar(config.CourseRun + ".tar.gz", ".");
			File.Delete(config.CourseRun + ".tar.gz");
			if (Directory.Exists(options.Dir + "/olx"))
				Directory.Delete(options.Dir + "/olx", true);
			Directory.Move(config.CourseRun, options.Dir + "/olx");
			
			var edxCourse = EdxCourse.Load(options.Dir + "/olx");

			var ulearnCourse = new CourseLoader().LoadCourse(new DirectoryInfo(string.Format("{0}/{1}", options.Dir, config.ULearnCourseId)));

			Console.WriteLine("Patching {0} with slides from {1}...", config.CourseRun, ulearnCourse.Id);
			edxCourse.PatchSlides(options.Dir + "/olx", 
				ulearnCourse.Id, 
				ulearnCourse.Slides, 
				config.ExerciseUrl, 
				config.SolutionsUrl, 
				config.LtiId, 
				options.ReplaceExisting, 
				options.Guids == null ? null : options.Guids.Split(',')
			);

			if (config.Video != null && File.Exists(string.Format("{0}/{1}", options.Dir, config.Video)))
			{
				var video = JsonConvert.DeserializeObject<Video>(File.ReadAllText(string.Format("{0}/{1}", options.Dir, config.Video)));
				edxCourse.PatchVideos(options.Dir + "/olx", 
					video.Records.ToDictionary(x => x.Guid.ToString("D"), x => Tuple.Create(x.Data.Id, x.Data.Name)), 
					options.Guids == null ? null : options.Guids.Split(',')
				);
			}

			Upload(options.Dir, edxCourse.CourseName, config, credentials);
		}

		private static void Upload(string baseDir, string courseName, Config config, Credentials credentials)
		{
			Environment.CurrentDirectory = baseDir;
			Utils.DeleteDirectoryIfExists("temp");
			if (Directory.Exists(courseName))
				Directory.Move(courseName, "temp");
			Utils.DirectoryCopy("olx", courseName, true);
			Utils.DeleteFileIfExists(courseName + ".tar.gz");

			Console.WriteLine("Creating {0}.tar.gz...", courseName);
			ArchiveManager.CreateTar(courseName + ".tar.gz", courseName);
			
			Utils.DeleteDirectoryIfExists(courseName);
			if (Directory.Exists("temp"))
				Directory.Move("temp", courseName);
			
			Console.WriteLine("Uploading {0}.tar.gz...", courseName);
			DownloadManager.Upload(config.Hostname, config.Port, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, courseName + ".tar.gz");
			Utils.DeleteFileIfExists(courseName + ".tar.gz");
		}

		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<StartOptions, ConvertOptions, PatchOptions>(args).Return(
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
