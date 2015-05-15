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

		public static readonly CourseSettings DefaultSettings = new CourseSettings(null, new Language("py", "3"));

		public CourseSettings()
		{
		}

		public CourseSettings(string title, params Language[] defaultLanguageVersions)
		{
			Title = title;
			DefaultLanguageVersions = defaultLanguageVersions;
		}

		public static CourseSettings Load(DirectoryInfo dir)
		{
			var file = dir.GetFile("Course.xml");
			if (!file.Exists)
				return DefaultSettings;
			var settings = file.DeserializeXml<CourseSettings>();
			if (settings.DefaultLanguageVersions == null)
				settings.DefaultLanguageVersions = new Language[0];
			return settings;
		}

		public string GetLanguageVersion(string language)
		{
			var res = DefaultLanguageVersions.FirstOrDefault(lang => lang.Name == language);
			if (res == null && Title != null)
				return DefaultSettings.GetLanguageVersion(language);
			return res == null ? null : res.Version;
		}
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