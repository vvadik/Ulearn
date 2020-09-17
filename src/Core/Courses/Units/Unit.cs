using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			Flashcards = new List<Flashcard>();
		}

		public UnitSettings Settings { get; set; }

		public InstructorNote InstructorNote { get; set; }

		private List<Slide> Slides { get; set; }

		private List<Slide> notHiddenSlides { get; set; }
		private List<Slide> NotHiddenSlides
		{
			get { return notHiddenSlides ??= Slides.Where(s => !s.Hide).ToList(); }
		}

		public List<Flashcard> Flashcards { get; set; }

		public DirectoryInfo Directory { get; set; }

		public Guid Id => Settings.Id;

		public string Title => Settings.Title;

		public string Url => Settings.Url;

		public ScoringSettings Scoring => Settings.Scoring;

		// Используется только в UnitLoader. Иначе в Course не обновится slidesCache
		public void SetSlides(List<Slide> slides)
		{
			Slides = slides;
			notHiddenSlides = null;
		}

		public List<Slide> GetSlides(bool withHidden)
		{
			if (withHidden)
				return Slides;
			return NotHiddenSlides;
		}
		
		public List<Slide> GetHiddenSlides()
		{
			return Slides.Where(s => s.Hide).ToList();
		}

		public void LoadInstructorNote(CourseLoadingContext context)
		{
			var instructorNoteFile = Directory.GetFile("InstructorNotes.md");
			if (instructorNoteFile.Exists)
			{
				InstructorNote = InstructorNote.Load(context, instructorNoteFile, this);
			}
		}

		public Flashcard GetFlashcardById(string id)
		{
			return Flashcards.FirstOrDefault(x => x.Id == id);
		}
	}
}