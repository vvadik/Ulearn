using System.IO;
using System.Linq;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.CSharp
{
	public class CSharpSlideLoader : ISlideLoader
	{
		public string Extension => ".cs";

		public Slide Load(FileInfo file, Unit unit, int slideIndex, string courseId, CourseSettings settings)
		{
			var prelude = GetPrelude(file.Directory);
			return SlideParser.ParseSlide(file, new SlideInfo(unit, file, slideIndex), prelude, file.Directory, settings);
		}

		private static string GetPrelude(DirectoryInfo dir)
		{
			var preludeFile = new[] { dir, dir.Parent }.
				SelectMany(d => d.GetFiles("prelude.*", SearchOption.TopDirectoryOnly))
				.FirstOrDefault(f => f.Exists);
			return preludeFile == null ? "" : preludeFile.ContentAsUtf8();
		}
	}
}