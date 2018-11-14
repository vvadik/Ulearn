using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;

namespace uLearn.Courses
{
	/*
	 * These settings are loading from course.xml from the root folder of course. 
	 */
	[XmlRoot("Course", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/course")]
	public class CourseSettings
	{
		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("language")]
		public Language[] DefaultLanguageVersions { get; set; }

		[XmlElement("video-annotations-google-doc")]
		public string VideoAnnotationsGoogleDoc { get; set; }

		[XmlElement("manual-checking")]
		public bool IsManualCheckingEnabled { get; set; }
		
		[XmlElement("scoring")]
		public ScoringSettings Scoring { get; set; }
		
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
			new[] { new Language("cs", "7"), new Language("py", "3") },
			new[] { new PreludeFile("cs", "Prelude.cs") },
			"dictionary.txt"
		);

		[XmlIgnore]
		private static readonly Regex scoringGroupIdRegex = new Regex("^[a-z0-9_]+$", RegexOptions.IgnoreCase);

		public CourseSettings()
		{
			Scoring = new ScoringSettings();
		}

		public CourseSettings(string title, Language[] defaultLanguageVersions, PreludeFile[] preludes, string dictionaryFile)
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
			DefaultLanguageVersions = (Language[])other.DefaultLanguageVersions.Clone();
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
				settings.DefaultLanguageVersions = new Language[0];
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

	public class ScoringSettings
	{
		public ScoringSettings()
		{
			_groups = new ScoringGroup[0];
		}

		[XmlAttribute("defaultQuiz")]
		public string _defaultScoringGroupForQuiz { get; set; }

		[XmlIgnore]
		public string DefaultScoringGroupForQuiz =>
			string.IsNullOrEmpty(_defaultScoringGroupForQuiz) ? DefaultScoringGroup : _defaultScoringGroupForQuiz;

		[XmlAttribute("defaultExercise")]
		public string _defaultScoringGroupForExercise { get; set; }

		[XmlIgnore]
		public string DefaultScoringGroupForExercise =>
			string.IsNullOrEmpty(_defaultScoringGroupForExercise) ? DefaultScoringGroup : _defaultScoringGroupForExercise;

		[XmlAttribute("default")]
		public string DefaultScoringGroup { get; set; }

		[XmlElement("group")]
		public ScoringGroup[] _groups { get; set; }

		private SortedDictionary<string, ScoringGroup> groupsCache;

		[XmlIgnore]
		public SortedDictionary<string, ScoringGroup> Groups
		{
			get { return groupsCache ?? (groupsCache = _groups.ToDictionary(g => g.Id, g => g).ToSortedDictionary()); }
		}

		public void CopySettingsFrom(ScoringSettings otherScoringSettings)
		{
			_defaultScoringGroupForQuiz = string.IsNullOrEmpty(_defaultScoringGroupForQuiz) && string.IsNullOrEmpty(DefaultScoringGroup)
				? otherScoringSettings.DefaultScoringGroupForQuiz
				: _defaultScoringGroupForQuiz;
			_defaultScoringGroupForExercise = string.IsNullOrEmpty(_defaultScoringGroupForExercise) && string.IsNullOrEmpty(DefaultScoringGroup)
				? otherScoringSettings.DefaultScoringGroupForExercise
				: _defaultScoringGroupForExercise;
			DefaultScoringGroup = string.IsNullOrEmpty(DefaultScoringGroup) ? otherScoringSettings.DefaultScoringGroup : DefaultScoringGroup;

			/* Copy missing scoring groups */
			foreach (var scoringGroupId in otherScoringSettings.Groups.Keys)
				if (!Groups.ContainsKey(scoringGroupId))
					Groups[scoringGroupId] = otherScoringSettings.Groups[scoringGroupId];
		}

		public int GetMaxAdditionalScore()
		{
			return Groups.Values.Where(g => g.CanBeSetByInstructor).Sum(g => g.MaxAdditionalScore);
		}
	}

	public class ScoringGroup
	{
		private const bool DefaultCanBeSetByInstructor = false;
		private const int DefaultMaxAdditionalScore = 10;
		private const bool DefaultEnabledForEveryone = false;

		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("abbr")]
		public string Abbreviation { get; set; }

		[XmlAttribute("description")]
		public string Description { get; set; }

		/* Hack to handle empty bool and integer attributes,
		 * because standard XmlSerializer doesn't work with nullable (i.e. int? and bool?) fields */
		[XmlAttribute("set_by_instructor")]
		public string _canBeSetByInstructor;

		[XmlIgnore]
		public bool CanBeSetByInstructor
		{
			get
			{
				if (string.IsNullOrEmpty(_canBeSetByInstructor) || _canBeSetByInstructor.Trim().Length == 0)
					return DefaultCanBeSetByInstructor;

				return bool.TryParse(_canBeSetByInstructor, out bool value) ? value : DefaultCanBeSetByInstructor;
			}
			set => _canBeSetByInstructor = value.ToString();
		}

		[XmlIgnore]
		public bool IsCanBeSetByInstructorSpecified => !string.IsNullOrEmpty(_canBeSetByInstructor);

		[XmlAttribute("max_additional_score")]
		public string _maxAdditionalScore { get; set; }

		[XmlIgnore]
		public int MaxAdditionalScore
		{
			get
			{
				if (string.IsNullOrEmpty(_maxAdditionalScore) || _maxAdditionalScore.Trim().Length == 0)
					return DefaultMaxAdditionalScore;

				int result;
				return int.TryParse(_maxAdditionalScore, out result) ? result : DefaultMaxAdditionalScore;
			}
			set => _maxAdditionalScore = value.ToString();
		}

		[XmlIgnore]
		/* Calculates automatically by slides's scores */
		public int MaxNotAdditionalScore { get; set; }

		[XmlIgnore]
		public bool IsMaxAdditionalScoreSpecified => !string.IsNullOrEmpty(_maxAdditionalScore);

		[XmlAttribute("enable_for_everyone")]
		public string _enabledForEveryone;

		[XmlIgnore]
		public bool EnabledForEveryone
		{
			get
			{
				if (string.IsNullOrEmpty(_enabledForEveryone) || _enabledForEveryone.Trim().Length == 0)
					return DefaultEnabledForEveryone;

				return bool.TryParse(_enabledForEveryone, out bool value) ? value : DefaultEnabledForEveryone;
			}
			set => _enabledForEveryone = value.ToString();
		}

		[XmlIgnore]
		public bool IsEnabledForEveryoneSpecified => !string.IsNullOrEmpty(_enabledForEveryone);

		[XmlText]
		public string Name { get; set; }

		public void CopySettingsFrom(ScoringGroup otherScoringGroup)
		{
			_canBeSetByInstructor = string.IsNullOrEmpty(_canBeSetByInstructor) ? otherScoringGroup._canBeSetByInstructor : _canBeSetByInstructor;
			_maxAdditionalScore = string.IsNullOrEmpty(_maxAdditionalScore) ? otherScoringGroup._maxAdditionalScore : _maxAdditionalScore;
			_enabledForEveryone = string.IsNullOrEmpty(_enabledForEveryone) ? otherScoringGroup._enabledForEveryone : _enabledForEveryone;
			Abbreviation = Abbreviation ?? otherScoringGroup.Abbreviation;
			Name = string.IsNullOrEmpty(Name) ? otherScoringGroup.Name : Name;
			Description = string.IsNullOrEmpty(Description) ? otherScoringGroup.Description : Description;
		}
	}
}