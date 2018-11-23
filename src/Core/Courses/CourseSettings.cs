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
	[XmlRoot("course", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class CourseSettings
	{
		[XmlAttribute("title")]
		public string Title { get; set; }

		[XmlElement("defaultLanguage")]
		public Language? DefaultLanguage { get; set; }

		[XmlElement("enableCodeReviewAndQuizManualCheckForEveryone")]
		public string VideoAnnotationsGoogleDoc { get; set; }

		[XmlElement("manualChecking")]
		public bool IsManualCheckingEnabled { get; set; }
		
		[XmlElement("scoring")]
		public ScoringSettings Scoring { get; set; }
		
		[XmlArray("units")]
		[XmlArrayItem("add")]
		public string[] UnitPaths { get; set; }
		
		[XmlArray("preludes")]
		[XmlArrayItem("prelude")]
		public PreludeFile[] Preludes { get; set; }

		[XmlElement("dictionary")]
		public string DictionaryFile { get; set; }
		
		public static readonly CourseSettings DefaultSettings = new CourseSettings(
			null,
			null,
			new PreludeFile[0], 
			"dictionary.txt"
		);

		[XmlIgnore]
		private static readonly Regex scoringGroupIdRegex = new Regex("^[a-z0-9_]+$", RegexOptions.IgnoreCase);

		public CourseSettings()
		{
			Scoring = new ScoringSettings();
		}

		public CourseSettings(string title, Language? defaultLanguage, PreludeFile[] preludes, string dictionaryFile)
			: this()
		{
			Title = title;
			DefaultLanguage = defaultLanguage;
			Preludes = preludes;
			DictionaryFile = dictionaryFile;
		}

		public CourseSettings(CourseSettings other)
			: this()
		{
			Title = other.Title;
			DefaultLanguage = other.DefaultLanguage;
			Preludes = (PreludeFile[])other.Preludes.Clone();
			DictionaryFile = other.DictionaryFile;
		}

		public static CourseSettings Load(DirectoryInfo dir)
		{
			var file = dir.GetFile("course.xml");
			if (!file.Exists)
				return new CourseSettings(DefaultSettings);

			var settings = file.DeserializeXml<CourseSettings>();
			if (settings.Preludes == null)
				settings.Preludes = new PreludeFile[0];

			foreach (var scoringGroup in settings.Scoring.Groups.Values)
			{
				if (!scoringGroupIdRegex.IsMatch(scoringGroup.Id))
					throw new CourseLoadingException(
						$"Некорректный идентификатор группы баллов <group id={scoringGroup.Id}> (файл course.xml). " +
						"Идентификатор группы баллов может состоить только из латинских букв, цифр и подчёркивания, а также не может быть пустым. " +
						"Понятное человеку название используйте в аббревиатуре и имени группы."
					);
				
				if (scoringGroup.IsMaxAdditionalScoreSpecified &&
					(!scoringGroup.IsCanBeSetByInstructorSpecified || !scoringGroup.CanBeSetByInstructor))
					throw new CourseLoadingException(
						$"Чтобы выставлять дополнительные баллы в группу {scoringGroup.Id}, установите у неё атрибут setByInstructor=\"true\" в настройках курса (файл course.xml). " +
						$"В противном случае атрибут maxAdditionalScore=\"{scoringGroup.MaxAdditionalScore}\" не действует."
					);

				if (!scoringGroup.CanBeSetByInstructor && scoringGroup.IsEnabledForEveryoneSpecified)
					throw new CourseLoadingException(
						$"Неправильные параметры для группы баллов {scoringGroup.Id} в файле course.xml. " +
						"Опция enableForEveryone доступна только при установке опции maxAdditionalScore."
					);
			}

			return settings;
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

		#region Equals
		
		protected bool Equals(PreludeFile other)
		{
			return Language == other.Language && string.Equals(File, other.File);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((PreludeFile)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Language * 397) ^ (File != null ? File.GetHashCode() : 0);
			}
		}
		
		#endregion
	}
}