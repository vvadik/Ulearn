using System.IO;

namespace uLearn.Quizes
{
	public class QuizSlideLoader : ISlideLoader
	{
		public string Extension { get { return "xml"; } }

		public Slide Load(FileInfo file, string unitName, int slideIndex)
		{
			var quiz = file.DeserializeXml<Quiz>();
			var slideInfo = new SlideInfo(unitName, slideIndex);
			return new QuizSlide(slideInfo, quiz);
		}
	}
}