using System.Collections.Generic;
using System.Linq;

namespace uLearn.Model
{
	public class BuildUpContext
	{
		public IFileSystem FileSystem { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		public Lesson Lesson { get; private set; }
		private List<RegionsExtractor> Extractors { get; set; }

		public BuildUpContext(IFileSystem fileSystem, CourseSettings courseSettings, Lesson lesson)
		{
			FileSystem = fileSystem;
			CourseSettings = courseSettings;
			Lesson = lesson;
			Extractors = new List<RegionsExtractor>();
		}

		public RegionsExtractor GetExtractor(string file, string langId, string code = null)
		{
			var extractor = Extractors.FirstOrDefault(regionsExtractor => regionsExtractor.file == file && regionsExtractor.langId == langId);
			if (extractor != null)
				return extractor;

			if (code == null)
				code = FileSystem.GetContent(file);
			extractor = new RegionsExtractor(code, langId, file);
			Extractors.Add(extractor);
			return extractor;
		}
	}
}