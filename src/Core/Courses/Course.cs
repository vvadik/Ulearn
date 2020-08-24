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
		public Course(string id, List<Unit> units, [NotNull]CourseSettings settings, DirectoryInfo courseDirectory, DirectoryInfo courseXmlDirectory)
		{
			Id = id;
			this.units = units;
			Settings = settings;
			CourseXmlDirectory = courseXmlDirectory;
			CourseDirectory = courseDirectory;
		}

		public string Id { get; set; }
		public string Title => Settings.Title;
		[NotNull]
		public CourseSettings Settings { get; private set; }
		public DirectoryInfo CourseXmlDirectory { get; set; }
		public DirectoryInfo CourseDirectory { get; set; }
		private List<Unit> units;

		private List<Slide> slidesCache { get; set; }

		private List<Slide> Slides
		{
			get { return slidesCache ??= units.SelectMany(u => u.GetSlides(true)).ToList(); }
		}

		private List<Slide> notHiddenSlidesCache { get; set; }
		private List<Slide> NotHiddenSlides
		{
			get { return notHiddenSlidesCache ??= units.SelectMany(u => u.GetSlides(false)).ToList(); }
		}

		public List<Slide> GetSlides(bool withHidden)
		{
			if (withHidden)
				return Slides;
			return NotHiddenSlides;
		}

		[CanBeNull]
		public Slide FindSlideById(Guid slideId, bool withHidden)
		{
			return (withHidden ? Slides : NotHiddenSlides).FirstOrDefault(x => x.Id == slideId);
		}

		[NotNull]
		public Slide GetSlideById(Guid slideId, bool withHidden)
		{
			var slide = FindSlideById(slideId, withHidden);
			if (slide == null)
				throw new NotFoundException($"No slide with id {slideId}");
			return slide;
		}

		[CanBeNull]
		public InstructorNote FindInstructorNoteById(Guid slideId)
		{
			var unitWithId = FindUnitByIdNotSafe(slideId);
			return unitWithId?.InstructorNote;
		}

		[CanBeNull]
		public Unit FindUnitById(Guid unitId, List<Guid> visibleUnits)
		{
			if (!visibleUnits.Contains(unitId))
				return null;
			return FindUnitByIdNotSafe(unitId);
		}

		[NotNull]
		public Unit GetUnitById(Guid unitId, List<Guid> visibleUnits)
		{
			var unit = FindUnitById(unitId, visibleUnits);
			if (unit == null)
				throw new NotFoundException($"No unit with id {unitId}");
			return unit;
		}

		[CanBeNull]
		public Unit FindUnitByIdNotSafe(Guid unitId)
		{
			return units.FirstOrDefault(x => x.Id == unitId);
		}

		[NotNull]
		public Unit GetUnitByIdNotSafe(Guid unitId)
		{
			var unit = FindUnitByIdNotSafe(unitId);
			if (unit == null)
				throw new NotFoundException($"No unit with id {unitId}");
			return unit;
		}

		public List<Unit> GetUnits(IEnumerable<Guid> visibleUnits)
		{
			var visibleUnitsSet = visibleUnits is HashSet<Guid> ? visibleUnits : visibleUnits.ToHashSet();
			return units.Where(u => visibleUnitsSet.Contains(u.Id)).ToList();
		}

		/*
		 * Возвращает все юниты курса без проверки, что у пользователя есть доступ к ним
		 */
		public List<Unit> GetUnitsNotSafe()
		{
			return units;
		}

		[CanBeNull]
		public Unit FindUnitBySlideId(Guid slideId, bool withHiddenSlides)
		{
			return units.FirstOrDefault(u => u.GetSlides(withHiddenSlides).Any(s => s.Id == slideId));
		}

		public override string ToString()
		{
			return $"Course(Id: {Id}, Title: {Title})";
		}
	}

	public class InstructorNote
	{
		public InstructorNote(string markdown, Unit unit, FileInfo file, CourseLoadingContext courseLoadingContext)
		{
			Markdown = markdown;
			Unit = unit;
			File = file;
			Slide = new Slide(new MarkdownBlock(Markdown) { Hide = true })
			{
				Id = Unit.Id,
				Title = "Заметки преподавателю",
				Hide = true
			};
			var slideLoadingContext = new SlideLoadingContext(courseLoadingContext, unit, file);
			Slide.BuildUp(slideLoadingContext);
			Slide.Validate(slideLoadingContext);
		}

		public static InstructorNote Load(CourseLoadingContext context, FileInfo file, Unit unit)
		{
			return new InstructorNote(file.ContentAsUtf8(), unit, file, context);
		}

		public string Markdown;
		public Unit Unit;
		public FileInfo File;
		public Slide Slide;
	}
}