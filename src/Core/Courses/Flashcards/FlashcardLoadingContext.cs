using System.IO;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Flashcards
{
	public class FlashcardLoadingContext
	{
		public string CourseId { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		
		public DirectoryInfo UnitDirectory { get; }
		public FileInfo FlashcardsFile { get; }
		public int SlideIndex { get; private set; }
		
		public Unit Unit { get; }

		public FlashcardLoadingContext(string courseId, Unit unit, CourseSettings courseSettings, FileInfo flashcardsFile, int slideIndex)
		{
			CourseId = courseId;
			
			Unit = unit;
			UnitDirectory = unit.Directory;
			
			CourseSettings = courseSettings;
			FlashcardsFile = flashcardsFile;
			SlideIndex = slideIndex;
		}

		public FlashcardLoadingContext(CourseLoadingContext courseLoadingContext, Unit unit, FileInfo slideFile, int slideIndex)
			: this(courseLoadingContext.CourseId, unit, courseLoadingContext.CourseSettings, slideFile, slideIndex)
		{
			
		}
	}
}