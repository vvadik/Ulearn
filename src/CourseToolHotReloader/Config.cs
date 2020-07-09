using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CourseToolHotReloader.Log;

namespace CourseToolHotReloader
{
	public interface IConfig
	{
		string Path { get; set; }
		Dictionary<string, string> CourseIds { get; set; }
		string CourseId { get; set; }
		string JwtToken { get; set; }
		public string BaseUrl { get; set; }
		public bool SendFullArchive { get; set; }
		public string PathToConfigFile { get; }
		public void Flush();
	}

	internal class Config : IConfig
	{
		public readonly string pathToConfigFile = $"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\config.json";

		public Config()
		{
			var fileConfigFormat = ReadConfig();
			JwtToken = fileConfigFormat.JwtToken;
			BaseUrl = fileConfigFormat.BaseUrl;
			CourseIds = fileConfigFormat.CourseIds;
			Path = Directory.GetCurrentDirectory();
		}

		public string BaseUrl { get; set; }
		public bool SendFullArchive { get; set; }
		public string Path { get; set; }
		public Dictionary<string, string> CourseIds { get; set; }
		public string JwtToken { get; set; }

		public string CourseId
		{
			get => CourseIds[Path];
			set => CourseIds[Path] = value;
		}

		private FileConfigFormat ReadConfig()
		{
			if (!File.Exists(pathToConfigFile))
			{
				CreateNewConfigFile();
			}

			using var streamReader = File.OpenText(pathToConfigFile);
			var json = streamReader.ReadToEnd();
			var jsonWithCorrectFileName = Regex.Replace(json, @"[^\\|\/](\\{1}|\/)[^\\|\/]", "\\\\");
			var fileConfigFormat = JsonSerializer.Deserialize<FileConfigFormat>(jsonWithCorrectFileName);
			return fileConfigFormat;
		}

		private void CreateNewConfigFile()
		{
			var fileConfigFormat = new FileConfigFormat();

			SaveConfigFile(fileConfigFormat);
		}

		public string PathToConfigFile => pathToConfigFile;

		public void Flush()
		{
			var fileConfigFormat = new FileConfigFormat
			{
				JwtToken = JwtToken,
				BaseUrl = BaseUrl,
				SendFullArchive = SendFullArchive,
				CourseIds = CourseIds
			};

			SaveConfigFile(fileConfigFormat);
		}

		private void SaveConfigFile(FileConfigFormat fileConfigFormat)
		{
			using var fileStream = File.Create(pathToConfigFile);
			var text = JsonSerializer.Serialize(fileConfigFormat, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			var info = new UTF8Encoding(true).GetBytes(text);
			fileStream.Write(info, 0, info.Length);
		}


		private class FileConfigFormat
		{
			public FileConfigFormat()
			{
				JwtToken = null;
				BaseUrl = "ulearn.me";
				SendFullArchive = false;
				CourseIds = new Dictionary<string, string>();
			}

			[JsonPropertyName("jwtToken")]
			public string JwtToken { get; set; }

			[JsonPropertyName("baseUrl")]
			public string BaseUrl { get; set; }

			[JsonPropertyName("sendFullArchive")]
			public bool SendFullArchive { get; set; }

			[JsonPropertyName("courseIds")]
			public Dictionary<string, string> CourseIds { get; set; }
		}
	}
}