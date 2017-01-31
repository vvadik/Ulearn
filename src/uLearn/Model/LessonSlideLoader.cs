using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	public class LessonSlideLoader : ISlideLoader
	{
		public string Extension => "lesson.xml";

		public Slide Load(FileInfo file, string unitName, int slideIndex, CourseSettings settings)
		{
			var lesson = file.DeserializeXml<Lesson>();
			var dir = file.Directory;
			var context = new BuildUpContext(dir, settings, lesson);
			var blocks = lesson.Blocks.SelectMany(b => b.BuildUp(context, ImmutableHashSet<string>.Empty)).ToList();
			var slideInfo = new SlideInfo(unitName, file, slideIndex);
			if (blocks.OfType<ExerciseBlock>().Any())
				return new ExerciseSlide(blocks, slideInfo, lesson.Title, Guid.Parse(lesson.Id));
			return new Slide(blocks, slideInfo, lesson.Title, Guid.Parse(lesson.Id));
		}
	}
}