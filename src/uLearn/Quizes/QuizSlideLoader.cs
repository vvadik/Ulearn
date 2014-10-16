using System.IO;
using System.Linq;

namespace uLearn.Quizes
{
	public class QuizSlideLoader : ISlideLoader
	{
		public string Extension { get { return ".quiz.xml"; } }

		public Slide Load(FileInfo file, string unitName, int slideIndex)
		{
			var quiz = file.DeserializeXml<Quiz>();
			int index = 1;
			foreach (var b in quiz.Blocks.OfType<AbstractQuestionBlock>())
			{
				b.QuestionIndex = index++;
			}
			var slideInfo = new SlideInfo(unitName, file, slideIndex);
			return new QuizSlide(slideInfo, quiz);
		}
	}
}