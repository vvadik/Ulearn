using System.Collections.Immutable;
using System.IO;
using System.Linq;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	public class LessonSlideLoader : ISlideLoader
	{
		public string Extension
		{
			get { return "lesson.xml"; }
		}

		public Slide Load(FileInfo file, string unitName, int slideIndex, CourseSettings settings)
		{
			var lesson = file.DeserializeXml<Lesson>();
			var fs = new FileSystem(file.Directory);
			var blocks = lesson.Blocks.SelectMany(b => b.BuildUp(fs, ImmutableHashSet<string>.Empty, settings, lesson)).ToList();
			var slideInfo = new SlideInfo(unitName, file, slideIndex);
			if (blocks.OfType<ExerciseBlock>().Any())
				return new ExerciseSlide(blocks, slideInfo, lesson.Title, lesson.Id);
			return new Slide(blocks, slideInfo, lesson.Title, lesson.Id);
		}
	}
}