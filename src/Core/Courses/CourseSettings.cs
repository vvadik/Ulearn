using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;

namespace Ulearn.Core.Courses
{
	/*
	 * These settings are loading from course.xml from the root folder of the course. 
	 */
	[XmlRoot("Course", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/course")]
	public class CourseSettings
	{
		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("language")]
		public CourseLanguage[] DefaultLanguageVersions { get; set; }

		[XmlElement("video-annotations-google-doc")]
		public string VideoAnnotationsGoogleDoc { get; set; }

		[XmlElement("manual-checking")]
		public bool IsManualCheckingEnabled { get; set; }
		
		[XmlElement("scoring")]
		public ScoringSettings Scoring { get; set; }
		
		[XmlIgnore]
		public Language DefaultLanguage
		{
			get
			{
				if (DefaultLanguageVersions == null || DefaultLanguageVersions.Length == 0)
					return this == DefaultSettings ? Language.CSharp : DefaultSettings.DefaultLanguage;
				
				return Language.CSharp;
				// return DefaultLanguageVersions[0].Name;
			}
		}

		[XmlElement("prelude")]
		public PreludeFile[] Preludes { get; set; }

		[XmlElement("dictionary")]
		public string DictionaryFile { get; set; }

		public static readonly CourseSettings DefaultSettings = new CourseSettings(
			null,
			new[] { new CourseLanguage("cs", "7"), new CourseLanguage("py", "3") },
			new PreludeFile[0], 
			"dictionary.txt"
		);

		[XmlIgnore]
		private static readonly Regex scoringGroupIdRegex = new Regex("^[a-z0-9_]+$", RegexOptions.IgnoreCase);

		public CourseSettings()
		{
			Scoring = new ScoringSettings();
		}

		public CourseSettings(string title, CourseLanguage[] defaultLanguageVersions, PreludeFile[] preludes, string dictionaryFile)
			: this()
		{
			Title = title;
			DefaultLanguageVersions = defaultLanguageVersions;
			Preludes = preludes;
			DictionaryFile = dictionaryFile;
		}

		public CourseSettings(CourseSettings other)
			: this()
		{
			Title = other.Title;
			DefaultLanguageVersions = (CourseLanguage[])other.DefaultLanguageVersions.Clone();
			Preludes = (PreludeFile[])other.Preludes.Clone();
			DictionaryFile = other.DictionaryFile;
		}

		public static CourseSettings Load(DirectoryInfo dir)
		{
			var file = dir.GetFile("Course.xml");
			if (!file.Exists)
				return new CourseSettings(DefaultSettings);

			var settings = file.DeserializeXml<CourseSettings>();
			if (settings.DefaultLanguageVersions == null)
				settings.DefaultLanguageVersions = new CourseLanguage[0];
			if (settings.Preludes == null)
				settings.Preludes = new PreludeFile[0];

			foreach (var scoringGroup in settings.Scoring.Groups.Values)
			{
				if (!scoringGroupIdRegex.IsMatch(scoringGroup.Id))
					throw new CourseLoadingException(
						$"Некорректный идентификатор группы баллов <group id={scoringGroup.Id}> (файл Course.xml). " +
						"Идентификатор группы баллов может состоить только из латинских букв, цифр и подчёркивания, а также не может быть пустым. Понятное человеку название используйте в аббревиатуре и имени группы."
					);

				if (scoringGroup.IsMaxAdditionalScoreSpecified &&
					(!scoringGroup.IsCanBeSetByInstructorSpecified || !scoringGroup.CanBeSetByInstructor))
					throw new CourseLoadingException(
						$"Чтобы выставлять дополнительные баллы в группу {scoringGroup.Id}, установите у неё атрибут set_by_instructor=\"true\" в настройках курса (файл Course.xml). " +
						$"В противном случае атрибут max_additional_score=\"{scoringGroup.MaxAdditionalScore}\" не действует."
					);

				if (!scoringGroup.CanBeSetByInstructor && scoringGroup.IsEnabledForEveryoneSpecified)
					throw new CourseLoadingException(
						$"Неправильные параметры для группы баллов {scoringGroup.Id} в файле Course.xml. " +
						"Опция enable_for_everyone доступна только при установке опции set_by_instructor=\"true\"."
					);
			}

			return settings;
		}

		public string GetLanguageVersion(string langId)
		{
			var res = DefaultLanguageVersions.FirstOrDefault(lang => lang.Name == langId);
			if (res == null && Title != null && this != DefaultSettings)
				return DefaultSettings.GetLanguageVersion(langId);
			return res?.Version;
		}

		public string GetPrelude(Language? language)
		{
			var prelude = Preludes.FirstOrDefault(file => file.Language == language);
			if (prelude == null && Title != null && this != DefaultSettings)
				return DefaultSettings.GetPrelude(language);
			return prelude?.File;
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

		public PreludeFile(Language language, string file)
		{
			Language = language;
			File = file;
		}

		[XmlAttribute("language")]
		public Language Language { get; set; }

		[XmlText]
		public string File { get; set; }
	}
}