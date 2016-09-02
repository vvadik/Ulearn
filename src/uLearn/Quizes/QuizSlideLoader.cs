using System.Collections.Immutable;
using System.IO;
using System.Linq;
using uLearn.Model;

namespace uLearn.Quizes
{
	public class QuizSlideLoader : ISlideLoader
	{
		public string Extension { get { return ".quiz.xml"; } }


		public Slide Load(FileInfo file, string unitName, int slideIndex, CourseSettings settings)
		{
			var quiz = file.DeserializeXml<Quiz>();
			BuildUp(quiz, file.Directory, settings);
			quiz.InitQuestionIndices();
			var slideInfo = new SlideInfo(unitName, file, slideIndex);
			return new QuizSlide(slideInfo, quiz);
		}

		public static void BuildUp(Quiz quiz, DirectoryInfo slideDir, CourseSettings settings)
		{
			var context = new BuildUpContext(slideDir, settings, null);
			var blocks = quiz.Blocks.SelectMany(b => b.BuildUp(context, ImmutableHashSet<string>.Empty));
			quiz.Blocks = blocks.ToArray();
		}
	}
}