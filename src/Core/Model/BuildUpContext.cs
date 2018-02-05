using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ulearn.Common.Extensions;

namespace uLearn.Model
{
	public class BuildUpContext
	{
		public DirectoryInfo Dir { get; }
		public string CourseId { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		public Unit Unit { get; }
		public Lesson Lesson { get; private set; }
		public string SlideTitle { get; private set; }
//		public HashSet<string> ZippedProjectExercises = new HashSet<string>();
		private List<RegionsExtractor> Extractors { get; }

		public BuildUpContext(Unit unit, CourseSettings courseSettings, Lesson lesson, string courseId, string slideTitle)
		{
			Dir = unit.Directory;
			CourseSettings = courseSettings;
			Unit = unit;
			Lesson = lesson;
			CourseId = courseId;
			SlideTitle = slideTitle;
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