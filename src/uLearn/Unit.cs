using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace uLearn
{
	public class Unit
	{
		public Unit(UnitSettings settings, DirectoryInfo directory)
		{
			Settings = settings;
			Directory = directory;
		}

		private UnitSettings Settings { get; set; }

		public InstructorNote InstructorNote { get; set; }

		public List<Slide> Slides { get; set; }

		public DirectoryInfo Directory { get; set; }

		public Guid Id => Settings.Id;

		public string Title => Settings.Title;

		public string Url => Settings.Url;

		public void LoadInstructorNote()
		{
			var instructorNoteFile = Directory.GetFile("InstructorNotes.md");
			if (instructorNoteFile.Exists)
			{
				InstructorNote = InstructorNote.Load(instructorNoteFile, this);
			}
		}
	}

	[XmlRoot("Unit", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/unit")]
	public class UnitSettings
	{
		[XmlElement("id")]
		public Guid Id { get; set; }

		[XmlElement("url")]
		public string Url { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		public static UnitSettings Load(FileInfo file)
		{
			var unitSettings = file.DeserializeXml<UnitSettings>();

			if (string.IsNullOrEmpty(unitSettings.Title))
				throw new CourseLoadingException($"Заголовок модуля не может быть пустым. Файл {file.FullName}");

			if (string.IsNullOrEmpty(unitSettings.Url))
				unitSettings.Url = unitSettings.Title.ToLatin();

			return unitSettings;
		}

		public static UnitSettings CreateByTitle(string title)
		{
			return new UnitSettings
			{
				Id = title.ToDeterministicGuid(),
				Url = title.ToLatin(),
				Title = title,
			};
		}
	}
}