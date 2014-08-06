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
	public class QuizBlock
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("text")]
		public string Text;
	}

	[XmlType("Choice")]
	public class ChoiceBlock : QuizBlock
	{
		[XmlAttribute("multiple")]
		public bool Multiple;

		[XmlAttribute("shuffle")]
		public bool Shuffle;

		[XmlElement("item")]
		public ChoiceItem[] Items;
	}

	[XmlType("isTrue")]
	public class IsTrueBlock : QuizBlock
	{
		[XmlAttribute("answer")]
		public bool Answer;
	}

	[XmlType("fillIn")]
	public class FillInBlock : QuizBlock
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
