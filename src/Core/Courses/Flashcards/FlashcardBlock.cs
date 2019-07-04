using System.IO;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Flashcards
{
	public class FlashcardBlock : SlideBlock
	{
		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new System.NotImplementedException();
		}
	}
}