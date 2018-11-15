using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;

namespace Ulearn.Core.Courses.Units
{
	[XmlRoot("Unit", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/unit")]
	public class UnitSettings
	{
		public UnitSettings()
		{
			Scoring = new ScoringSettings();
		}

		[XmlElement("id")]
		public Guid Id { get; set; }

		[XmlElement("url")]
		public string Url { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("scoring")]
		public ScoringSettings Scoring { get; set; }

		[XmlElement("default-include-code-file")]
		public string DefaultIncludeCodeFile { get; set; }

		public static UnitSettings Load(FileInfo file, CourseSettings courseSettings)
		{
			var unitSettings = file.DeserializeXml<UnitSettings>();

			if (string.IsNullOrEmpty(unitSettings.Title))
				throw new CourseLoadingException($"Заголовок модуля не может быть пустым. Файл {file.FullName}");

			if (string.IsNullOrEmpty(unitSettings.Url))
				unitSettings.Url = unitSettings.Title.ToLatin();

			var courseScoringGroupsIds = new HashSet<string>(courseSettings.Scoring.Groups.Keys);
			foreach (var scoringGroup in unitSettings.Scoring.Groups.Values.ToList())
			{
				if (!courseScoringGroupsIds.Contains(scoringGroup.Id))
					throw new CourseLoadingException(
						$"Неизвестная группа баллов описана в модуле: {scoringGroup.Id}. Файл {file.FullName}. " +
						$"Для курса определены только следующие группы баллов: {string.Join(", ", courseScoringGroupsIds)}"
					);

				/* By default set unit's scoring settings from course's scoring settings */
				var unitScoringGroup = unitSettings.Scoring.Groups[scoringGroup.Id];
				var courseScoringGroup = courseSettings.Scoring.Groups[scoringGroup.Id];
				unitScoringGroup.CopySettingsFrom(courseScoringGroup);

				if (scoringGroup.IsEnabledForEveryoneSpecified)
					throw new CourseLoadingException(
						$"В настройках модуля «{unitSettings.Title}» для группы баллов {scoringGroup.Id} указана опция enable_for_everyone=\"{scoringGroup._enabledForEveryone}\" (файл {file.Directory?.Name}/Unit.xml). " +
						"Эту опцию можно указывать только в настройках всего курса (файл Course.xml)");
			}

			/* Copy other scoring groups and scoring settings from course settings */
			unitSettings.Scoring.CopySettingsFrom(courseSettings.Scoring);

			return unitSettings;
		}

		public static UnitSettings CreateByTitle(string title, CourseSettings courseSettings)
		{
			var unitSettings = new UnitSettings
			{
				/* We use Win1251 only for back compatibility.
				   In future all units will have Unit.xml with Id specified, so we will be able to switch to Encoding.UTF8 here or remove this function. */
				Id = title.ToDeterministicGuid(Encoding.GetEncoding(1251)),
				Url = title.ToLatin(),
				Title = title,
			};

			unitSettings.Scoring.CopySettingsFrom(courseSettings.Scoring);
			return unitSettings;
		}
	}
}