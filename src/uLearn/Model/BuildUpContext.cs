using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uLearn.Model
{
	public class BuildUpContext
	{
		public DirectoryInfo Dir { get; }
		public CourseSettings CourseSettings { get; private set; }
		public Lesson Lesson { get; private set; }
		public HashSet<string> ZippedProjectExercises = new HashSet<string>();
		private List<RegionsExtractor> Extractors { get; }

		public BuildUpContext(DirectoryInfo dir, CourseSettings courseSettings, Lesson lesson)
		{
			Dir = dir;
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
				code = Dir.GetContent(file);
			extractor = new RegionsExtractor(code, langId, file);
			Extractors.Add(extractor);
			return extractor;
		}
	}
}