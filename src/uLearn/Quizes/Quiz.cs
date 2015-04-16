using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace uLearn.Quizes
{
	[XmlRootAttribute("Quiz", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/quiz")]
	public class Quiz
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlAttribute("maxDropCount")]
		public int MaxDropCount;

		[XmlElement("title")]
		public string Title;

		private QuizBlock[] blocks;

		[XmlElement("p", Type = typeof(TextBlock))]
		[XmlElement("code", Type = typeof(CodeBlock))]
		[XmlElement("isTrue", Type = typeof(IsTrueBlock))]
		[XmlElement("choice", Type = typeof(ChoiceBlock))]
		[XmlElement("fillIn", Type = typeof(FillInBlock))]
		public QuizBlock[] Blocks
		{
			get { return blocks ?? new QuizBlock[0]; }
			set { blocks = value; }
		}

		public void Validate()
		{
			try
			{
				foreach (var quizBlock in Blocks)
					quizBlock.Validate();
			}
			catch (Exception e)
			{
				throw new FormatException("QuizId=" + Id, e);
			}
		}
	}

	public abstract class QuizBlock
	{
		public abstract void Validate();
	}
	
	public class TextBlock : QuizBlock
	{
		[XmlText]
		public string Text;

		public override void Validate()
		{
			if (string.IsNullOrEmpty(Text)) 
				throw new FormatException("Quiz TextBlock can't be empty");
		}
	}

	public class CodeBlock : QuizBlock
	{
		[XmlText]
		public string Text;
		[XmlAttribute("lang")]
		public string Lang;

		public override void Validate()
		{
			if (string.IsNullOrEmpty(Text))
				throw new FormatException("Quiz TextBlock can't be empty");
		}
	}

	public abstract class AbstractQuestionBlock : QuizBlock
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
	
		public override void Validate()
		{
			var correctCount = Items.Count(i => i.IsCorrect);
			if (!Multiple && correctCount != 1)
				throw new FormatException("Should be exaclty one correct item for non-multiple choice. BlockId=" + Id);
		}
	}

	public class IsTrueBlock : AbstractQuestionBlock
	{
		[XmlAttribute("answer")]
		public bool Answer;

		[XmlAttribute("explanation")]
		public string Explanation;

		public bool IsRight(string text)
		{
			return text.ToLower() == Answer.ToString().ToLower();
		}
		public override void Validate()
		{
		}
	}

	public class FillInBlock : AbstractQuestionBlock
	{
		[XmlElement("sample")]
		public string Sample;

		[XmlElement("regex")]
		public RegexInfo[] Regexes;

		[XmlAttribute("explanation")]
		public string Explanation;

		public override void Validate()
		{
			if (!Regexes.Any(re => re.Regex.IsMatch(Sample)))
				throw new FormatException("Sample should match at least one regex. BlockId=" + Id);
		}
	}

	public class RegexInfo
	{
		[XmlText]
		public string Pattern;

		[XmlAttribute("ignoreCase")]
		public bool IgnoreCase;

		private Regex regex;

		public Regex Regex
		{
			get { return regex ?? (regex = new Regex("^" + Pattern + "$", IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)); }
		}
	}

	public class ChoiceItem
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlAttribute("isCorrect")]
		public bool IsCorrect;

		[XmlAttribute("explanation")]
		public string Explanation;

		[XmlText]
		public string Description;
	}
}
