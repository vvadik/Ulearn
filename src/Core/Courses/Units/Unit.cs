using System;
using System.Collections.Generic;
using System.IO;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Flashcards;

namespace Ulearn.Core.Courses.Units
{
	public class Unit
	{
		public Unit(UnitSettings settings, DirectoryInfo directory)
		{
			Settings = settings;
			Directory = directory;
		}

		public UnitSettings Settings { get; set; }

		public InstructorNote InstructorNote { get; set; }

		public List<Slide> Slides { get; set; }

		public Dictionary<string, Flashcard> Flashcards { get; set; }
		
		public DirectoryInfo Directory { get; set; }

		public Guid Id => Settings.Id;

		public string Title => Settings.Title;

		public string Url => Settings.Url;

		public ScoringSettings Scoring => Settings.Scoring;

		public void LoadInstructorNote()
		{
			var instructorNoteFile = Directory.GetFile("InstructorNotes.md");
			if (instructorNoteFile.Exists)
			{
				InstructorNote = InstructorNote.Load(instructorNoteFile, this);
			}
		}
		
		public Flashcard GetFlashcardById(string id)
		{
			Flashcards.TryGetValue(id, out var flashcard);
			return flashcard;
		}

	}
}