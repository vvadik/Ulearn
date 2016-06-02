using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uLearn
{
	public class Course
	{
		public Course(string id, string title, Slide[] slides, InstructorNote[] instructorNotes, CourseSettings settings, DirectoryInfo directory)
		{
			Id = id;
			Title = title;
			Slides = slides;
			InstructorNotes = instructorNotes;
			Settings = settings;
			Directory = directory;
		}

		public string Id { get; set; }
		public string Title { get; }
		public Slide[] Slides { get; }
		public InstructorNote[] InstructorNotes { get; }
		public CourseSettings Settings { get; private set; }
		public DirectoryInfo Directory { get; private set; }

		public string GetDirectoryByUnitName(string unitName)
		{
			return Slides.First(x => x.Info.UnitName == unitName).Info.Directory.FullName;
		}

		[CanBeNull]
		public Slide FindSlideById(Guid slideId)
		{
			return Slides.FirstOrDefault(x => x.Id == slideId);
		}

		[NotNull]
		public Slide GetSlideById(Guid slideId)
		{
			var slide = Slides.FirstOrDefault(x => x.Id == slideId);
			if (slide == null)
				throw new Exception($"No slide with id {slideId}");
			return slide;
		}

		[CanBeNull]
		public Slide FindSlide(int index)
		{
			return index >= 0 && index < Slides.Length ? Slides[index] : null;
		}

		public int GetSlideIndexById(Guid slideId)
		{
			return Slides.FindIndex(x => x.Id == slideId);
		}

		public IEnumerable<string> GetUnits()
		{
			return Slides.Select(s => s.Info.UnitName).Distinct();
		}

		public InstructorNote FindInstructorNote(string unitName)
		{
			return InstructorNotes.SingleOrDefault(n => n.UnitName == unitName);
		}

		public override string ToString()
		{
			return string.Format("Id: {0}, Title: {1}", Id, Title);
		}
	}

	public class InstructorNote
	{
		public InstructorNote(string markdown, string courseId, string unitName, FileInfo file)
		{
			Markdown = markdown;
			CourseId = courseId;
			UnitName = unitName;
			File = file;
		}

		public string Markdown;
		public string CourseId;
		public string UnitName;
		public FileInfo File;
	}
}