using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Units;
using Ulearn.Core.Model;

namespace Ulearn.Core.Courses.Slides
{
	public class CourseLoadingContext
	{
		public string CourseId { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		
		public DirectoryInfo UnitDirectory { get; }
		public FileInfo SlideFile { get; }
		public int SlideIndex { get; private set; }
		
		public Unit Unit { get; }

		public CourseLoadingContext(string courseId, Unit unit, CourseSettings courseSettings, FileInfo slideFile, int slideIndex)
		{
			CourseId = courseId;
			
			Unit = unit;
			UnitDirectory = unit.Directory;
			
			CourseSettings = courseSettings;
			SlideFile = slideFile;
			SlideIndex = slideIndex;
		}
	}

	public class SlideLoadingContext
	{
		public string CourseId { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		
		public DirectoryInfo UnitDirectory { get; }
		public Slide Slide { get; set; }
		
		public Unit Unit { get; }
		
		private List<RegionsExtractor> Extractors { get; }

		public SlideLoadingContext(string courseId, Unit unit, CourseSettings courseSettings, Slide slide)
		{
			CourseId = courseId;
			
			Unit = unit;
			UnitDirectory = unit.Directory;
			
			CourseSettings = courseSettings;
			Slide = slide;
			
			Extractors = new List<RegionsExtractor>();
		}

		public SlideLoadingContext(CourseLoadingContext courseContext, Slide slide)
			:this(courseContext.CourseId, courseContext.Unit, courseContext.CourseSettings, slide)
		{
		}

		public RegionsExtractor GetExtractor(string filename, Language? language, string code = null)
		{
			var extractor = Extractors.FirstOrDefault(regionsExtractor => regionsExtractor.Filename == filename && regionsExtractor.Language == language);
			if (extractor != null)
				return extractor;

			if (code == null)
				code = UnitDirectory.GetContent(filename);
			extractor = new RegionsExtractor(code, language, filename);
			Extractors.Add(extractor);
			return extractor;
		}
	}
}