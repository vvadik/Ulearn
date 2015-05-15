using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uLearn
{
	public class Course
	{
		public Course(string id, string title, Slide[] slides, InstructorNote[] instructorNotes, CourseSettings settings)
		{
			Id = id;
			Title = title;
			Slides = slides;
			InstructorNotes = instructorNotes;
			Settings = settings;
		}

		public string Id { get; private set; }
		public string Title { get; private set; }
		public Slide[] Slides { get; private set; }
		public InstructorNote[] InstructorNotes { get; private set; }
		public CourseSettings Settings { get; private set; }

		public Slide GetSlideById(string slideId)
		{
			return Slides.FirstOrDefault(x => x.Id == slideId);
		}
		
		public Slide FindSlide(int index)
		{
			return index >= 0 && index < Slides.Length ? Slides[index] : null;
		}

		public IEnumerable<string> GetUnits()
		{
			return Slides.Select(s => s.Info.UnitName).Distinct();
		}

		public InstructorNote GetInstructorNote(string unitName)
		{
			return InstructorNotes.SingleOrDefault(n => n.UnitName == unitName);
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