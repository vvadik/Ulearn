using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

		[XmlElement("p")]
		public QuizBlock[] Blocks;
	}

	[XmlInclude(typeof(IsTrueBlock))]
	[XmlInclude(typeof(ChoiceBlock))]
	[XmlInclude(typeof(FillInBlock))]
	[XmlInclude(typeof(CodeBlock))]
	public class QuizBlock
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("text")]
		public string Text;
	}

	[XmlType("Code")]
	public class CodeBlock : QuizBlock
	{
	}

	public class AbstractQuestionBlock : QuizBlock
	{
		[XmlIgnore] public int QuestionIndex;
	}

	[XmlType("Choice")]
	public class ChoiceBlock : AbstractQuestionBlock
	{
		[XmlAttribute("multiple")]
		public bool Multiple;

		[XmlAttribute("shuffle")]
		public bool Shuffle;

		[XmlElement("item")]
		public ChoiceItem[] Items;
	}

	[XmlType("isTrue")]
	public class IsTrueBlock : AbstractQuestionBlock
	{
		[XmlAttribute("answer")]
		public bool Answer;

		public bool IsRight(string text)
		{
			return text.ToLower() == Answer.ToString().ToLower();
		}
	}

	[XmlType("fillIn")]
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
