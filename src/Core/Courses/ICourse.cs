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
		List<Unit> GetUnits([NotNull]IEnumerable<Guid> visibleUnits);
		List<Unit> GetUnitsNotSafe();

		Slide FindSlideById(Guid slideId, bool withHidden);
		Slide GetSlideById(Guid slideId, bool withHidden);
		Unit FindUnitById(Guid unitId, [NotNull] List<Guid> visibleUnits);
		Unit GetUnitById(Guid unitId, [NotNull] List<Guid> visibleUnits);
		Unit FindUnitByIdNotSafe(Guid unitId);
		Unit GetUnitByIdNotSafe(Guid unitId);
		Unit FindUnitBySlideIdNotSafe(Guid slideId, bool withHidden);
		Slide FindInstructorNoteByIdNotSafe(Guid slideId);
	}
}