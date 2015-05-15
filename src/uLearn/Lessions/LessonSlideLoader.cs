using System.Collections.Immutable;
using System.IO;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Lessions
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
			var blocks = lesson.Blocks.SelectMany(b => b.BuildUp(fs, ImmutableHashSet<string>.Empty, settings));
			return new Slide(blocks, new SlideInfo(unitName, file, slideIndex), lesson.Title, lesson.Id);
		}
	}
}