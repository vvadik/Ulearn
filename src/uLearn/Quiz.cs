using System.Xml.Serialization;

namespace uLearn.Quizes
{
	[XmlRootAttribute("Quiz", IsNullable = false)]
	public class Quiz
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("title")]
		public string Title;

		[XmlElement("p", Type = typeof(TextBlock))]
		[XmlElement("code", Type = typeof(CodeBlock))]
		[XmlElement("isTrue", Type = typeof(IsTrueBlock))]
		[XmlElement("choice", Type = typeof(ChoiceBlock))]
		[XmlElement("fillIn", Type = typeof(FillInBlock))]
		public QuizBlock[] Blocks;
	}

	public abstract class QuizBlock
	{
	}
	
	public class TextBlock : QuizBlock
	{
		[XmlText]
		public string Text;
	}

	public class CodeBlock : QuizBlock
	{
		[XmlText]
		public string Text;
	}

	public class AbstractQuestionBlock : QuizBlock
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("text")]
		public string Text;

		[XmlIgnore]
		public int QuestionIndex;
	}

	public class ChoiceBlock : AbstractQuestionBlock
	{
		[XmlAttribute("multiple")]
		public bool Multiple;

		[XmlAttribute("shuffle")]
		public bool Shuffle;

		[XmlElement("item")]
		public ChoiceItem[] Items;
	}

	public class IsTrueBlock : AbstractQuestionBlock
	{
		[XmlAttribute("answer")]
		public bool Answer;

		public bool IsRight(string text)
		{
			return text.ToLower() == Answer.ToString().ToLower();
		}
	}

	public class FillInBlock : AbstractQuestionBlock
	{
		[XmlElement("sample")]
		public string Sample;

		[XmlElement("regex")]
		public string[] Regexes;
	}

	public class ChoiceItem
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlAttribute("isCorrect")]
		public bool IsCorrect;

		[XmlText]
		public string Description;
	}
}
