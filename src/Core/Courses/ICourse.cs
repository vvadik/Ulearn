using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses
{
	public interface ICourse
	{
		string Id { get; set; }
		string Title { get; }
		CourseSettings Settings { get; }
		DirectoryInfo CourseXmlDirectory { get; }
		List<Unit> GetUnits([NotNull]IEnumerable<Guid> visibleUnits);
		List<Unit> GetUnitsNotSafe();
		List<Slide> Slides { get; }

		Slide FindSlideById(Guid slideId);
		Slide GetSlideById(Guid slideId);
		Slide FindSlideByIndex(int index);
		Unit FindUnitById(Guid unitId, [NotNull] List<Guid> visibleUnits);
		Unit GetUnitById(Guid unitId, [NotNull] List<Guid> visibleUnits);
		Unit FindUnitByIdNotSafe(Guid unitId);
		Unit GetUnitByIdNotSafe(Guid unitId);
		Unit FindUnitBySlideId(Guid slideId);
		InstructorNote FindInstructorNoteById(Guid slideId);
	}
}