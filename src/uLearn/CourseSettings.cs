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

		[XmlElement("manual-checking")]
		public bool IsManualCheckingEnabled { get; set; }

		[XmlElement("scoring")]
		public CourseScoring Scoring { get; set; }

		[XmlIgnore]
		public string DefaultLanguage
		{
			get
			{
				if (DefaultLanguageVersions == null || DefaultLanguageVersions.Length == 0)
					return this == DefaultSettings ? "cs" : DefaultSettings.DefaultLanguage;
				return DefaultLanguageVersions[0].Name;
			}
		}

		[XmlElement("prelude")]
		public PreludeFile[] Preludes { get; set; }

		[XmlElement("dictionary")]
		public string DictionaryFile { get; set; }

		public static readonly CourseSettings DefaultSettings = new CourseSettings(
			null,
			new[] { new Language("cs", "6"), new Language("py", "3") },
			new[] { new PreludeFile("cs", "Prelude.cs") },
			"dictionary.txt"
		);

		public CourseSettings()
		{
			Scoring = new CourseScoring();
		}

		public CourseSettings(string title, Language[] defaultLanguageVersions, PreludeFile[] preludes, string dictionaryFile)
		{
			Title = title;
			DefaultLanguageVersions = defaultLanguageVersions;
			Preludes = preludes;
			DictionaryFile = dictionaryFile;
			Scoring = new CourseScoring();
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
			if (res == null && Title != null && this != DefaultSettings)
				return DefaultSettings.GetLanguageVersion(langId);
			return res?.Version;
		}

		public string GetPrelude(string langId)
		{
			var res = Preludes.FirstOrDefault(file => file.LangId == langId);
			if (res == null && Title != null && this != DefaultSettings)
				return DefaultSettings.GetPrelude(langId);
			return res?.File;
		}

		public string GetDictionaryFile()
		{
			return DictionaryFile ?? DefaultSettings.DictionaryFile;
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

	public class CourseScoring
	{
		public CourseScoring()
		{
			Groups = new CourseScoringGroup[0];
		}

		[XmlAttribute("defaultQuiz")]
		private string defaultScoringGroupForQuiz { get; set; }

		[XmlIgnore]
		public string DefaultScoringGroupForQuiz =>
			string.IsNullOrEmpty(defaultScoringGroupForQuiz) ? DefaultScoringGroup : defaultScoringGroupForQuiz;

		[XmlAttribute("defaultExercise")]
		private string defaultScoringGroupForExercise { get; set; }

		[XmlIgnore]
		public string DefaultScoringGroupForExercise =>
			string.IsNullOrEmpty(defaultScoringGroupForExercise) ? DefaultScoringGroup : defaultScoringGroupForExercise;

		[XmlAttribute("default")]
		public string DefaultScoringGroup { get; set; }

		public CourseScoringGroup[] Groups { get; set; }
	}

	public class CourseScoringGroup
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("abbr")]
		public string Abbreviation { get; set; }

		[XmlAttribute("set_by_instructor")]
		public bool CanBeSetByInstructor { get; set; }

		[XmlText]
		public string Name { get; set; }
	}
}