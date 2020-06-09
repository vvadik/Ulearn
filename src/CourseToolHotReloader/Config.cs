using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourseToolHotReloader
{
	public interface IConfig
	{
		string Path { get; set; }
		string CourseId { get; set; }
		string JwtToken { get; set; }
		public string BaseUrl { get; set; }
		public bool SendFullArchive { get; set; }
		public void Flush();
	}

	internal class Config : IConfig
	{
		private readonly string pathToConfigFile = $"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\config.json";

		public Config()
		{
			var fileConfigFormat = ReadConfig();
			JwtToken = fileConfigFormat.JwtToken;
			BaseUrl = fileConfigFormat.BaseUrl;
			CourseId = fileConfigFormat.CourseId;
			Path = Directory.GetCurrentDirectory();
		}

		public string BaseUrl { get; set; }
		public bool SendFullArchive { get; set; }
		public string Path { get; set; }
		public string CourseId { get; set; }
		public string JwtToken { get; set; }

		private FileConfigFormat ReadConfig()
		{
			if (!File.Exists(pathToConfigFile))
			{
				CreateNewConfigFile();
			}

			using var streamReader = File.OpenText(pathToConfigFile);
			var json = streamReader.ReadToEnd();
			var fileConfigFormat = JsonSerializer.Deserialize<FileConfigFormat>(json);
			return fileConfigFormat;
		}

		private void CreateNewConfigFile()
		{
			var fileConfigFormat = new FileConfigFormat();

			SaveConfigFile(fileConfigFormat);
		}

		public void Flush()
		{
			var fileConfigFormat = new FileConfigFormat
			{
				JwtToken = JwtToken,
				BaseUrl = BaseUrl,
				SendFullArchive = SendFullArchive,
				CourseId = CourseId
			};

			SaveConfigFile(fileConfigFormat);
		}

		private void SaveConfigFile(FileConfigFormat fileConfigFormat)
		{
			using var fileStream = File.Create(pathToConfigFile);
			var text = JsonSerializer.Serialize(fileConfigFormat);
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
				CourseId = null;
			}

			[JsonPropertyName("jwtToken")]
			public string JwtToken { get; set; }

			[JsonPropertyName("baseUrl")]
			public string BaseUrl { get; set; }

			[JsonPropertyName("sendFullArchive")]
			public bool SendFullArchive { get; set; }

			[JsonPropertyName("courseId")]
			public string CourseId { get; set; }
		}
	}
}