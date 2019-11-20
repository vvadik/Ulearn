using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses
{
	public class Course : ICourse
	{
		public Course(string id, List<Unit> units, [NotNull]CourseSettings settings, DirectoryInfo courseXmlDirectory)
		{
			Id = id;
			Units = units;
			Settings = settings;
			CourseXmlDirectory = courseXmlDirectory;
		}

		public string Id { get; set; }
		public string Title => Settings.Title;
		[NotNull]
		public CourseSettings Settings { get; private set; }
		public DirectoryInfo CourseXmlDirectory { get; set; }
		public List<Unit> Units { get; private set; }

		private List<Slide> slidesCache { get; set; }

		public List<Slide> Slides
		{
			get { return slidesCache ?? (slidesCache = Units.SelectMany(u => u.Slides).ToList()); }
		}

		[CanBeNull]
		public Slide FindSlideById(Guid slideId)
		{
			return Slides.FirstOrDefault(x => x.Id == slideId);
		}

		[NotNull]
		public Slide GetSlideById(Guid slideId)
		{
			var slide = FindSlideById(slideId);
			if (slide == null)
				throw new NotFoundException($"No slide with id {slideId}");
			return slide;
		}

		[CanBeNull]
		public InstructorNote FindInstructorNoteById(Guid slideId)
		{
			var unitWithId = FindUnitById(slideId);
			return unitWithId?.InstructorNote;
		}

		[CanBeNull]
		public Unit FindUnitById(Guid unitId)
		{
			return Units.FirstOrDefault(x => x.Id == unitId);
		}

		[NotNull]
		public Unit GetUnitById(Guid unitId)
		{
			var unit = FindUnitById(unitId);
			if (unit == null)
				throw new NotFoundException($"No unit with id {unitId}");
			return unit;
		}

		[CanBeNull]
		public Unit FindUnitBySlideId(Guid slideId)
		{
			return Units.FirstOrDefault(u => u.Slides.Any(s => s.Id == slideId));
		}

		[CanBeNull]
		public Slide FindSlideByIndex(int index)
		{
			return index >= 0 && index < Slides.Count ? Slides[index] : null;
		}

		public override string ToString()
		{
			return $"Course(Id: {Id}, Title: {Title})";
		}
	}

	public class InstructorNote
	{
		public InstructorNote(string markdown, Unit unit, FileInfo file, CourseLoadingContext courseLoadingContext, int slideIndex)
		{
			Markdown = markdown;
			Unit = unit;
			File = file;
			Slide = new Slide(new MarkdownBlock(Markdown) { Hide = true })
			{
				Id = Unit.Id,
				Title = "Заметки преподавателю"
			};
			var slideLoadingContext = new SlideLoadingContext(courseLoadingContext, unit, file, slideIndex);
			Slide.BuildUp(slideLoadingContext);
			Slide.Validate(slideLoadingContext);
		}

		public static InstructorNote Load(CourseLoadingContext context, FileInfo file, Unit unit, int slideIndex)
		{
			return new InstructorNote(file.ContentAsUtf8(), unit, file, context, slideIndex);
		}

		public string Markdown;
		public Unit Unit;
		public FileInfo File;
		public Slide Slide;
	}
}