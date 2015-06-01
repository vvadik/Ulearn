using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace uLearn
{

	[XmlRoot("Course", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/course")]
	public class CourseSettings
	{
		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("language")]
		public Language[] DefaultLanguageVersions { get; set; }

		[XmlElement("prelude")]
		public PreludeFile[] Preludes { get; set; }

		public static readonly CourseSettings DefaultSettings = new CourseSettings(
			null, 
			new[] { new Language("py", "3") }, 
			new [] {new PreludeFile("cs", "Prelude.cs")}
		);

		public CourseSettings()
		{
		}

		public CourseSettings(string title, Language[] defaultLanguageVersions, PreludeFile[] preludes)
		{
			Title = title;
			DefaultLanguageVersions = defaultLanguageVersions;
			Preludes = preludes;
		}

		public static CourseSettings Load(DirectoryInfo dir)
		{
			var file = dir.GetFile("Course.xml");
			if (!file.Exists)
				return DefaultSettings;
			var settings = file.DeserializeXml<CourseSettings>();
			if (settings.DefaultLanguageVersions == null)
				settings.DefaultLanguageVersions = new Language[0];
			if (settings.Preludes == null)
				settings.Preludes = new PreludeFile[0];
			return settings;
		}

		public string GetLanguageVersion(string langId)
		{
			var res = DefaultLanguageVersions.FirstOrDefault(lang => lang.Name == langId);
			if (res == null && Title != null)
				return DefaultSettings.GetLanguageVersion(langId);
			return res == null ? null : res.Version;
		}

		public string GetPrelude(string langId)
		{
			var res = Preludes.FirstOrDefault(file => file.LangId == langId);
			if (res == null && Title != null)
				return DefaultSettings.GetPrelude(langId);
			return res == null ? null : res.File;
		}
	}

	public class PreludeFile
	{
		public PreludeFile()
		{
		}

		public PreludeFile(string langId, string file)
		{
			LangId = langId;
			File = file;
		}

		[XmlAttribute("language")]
		public string LangId { get; set; }

		[XmlText]
		public string File { get; set; }
	}


	public class Language
	{
		public Language()
		{
		}

		public Language(string name, string version)
		{
			Name = name;
			Version = version;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("version")]
		public string Version { get; set; }
	}
}