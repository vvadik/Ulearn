using System;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web.Services.Description;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.Model.Blocks;
using uLearn.Model.EdxComponents;

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

		private SlideBlock[] blocks;

		[XmlElement("p", Type = typeof(MdBlock))]
		[XmlElement("code", Type = typeof(CodeBlock))]
		[XmlElement("isTrue", Type = typeof(IsTrueBlock))]
		[XmlElement("choice", Type = typeof(ChoiceBlock))]
		[XmlElement("fillIn", Type = typeof(FillInBlock))]
		public SlideBlock[] Blocks
		{
			get { return blocks ?? new SlideBlock[0]; }
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
	
	public abstract class AbstractQuestionBlock : SlideBlock
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

		public ChoiceItem[] ShuffledItems()
		{
			return Shuffle ? Items.Shuffle().ToArray() : Items;
		}

		public override void Validate()
		{
			var correctCount = Items.Count(i => i.IsCorrect);
			if (!Multiple && correctCount != 1)
				throw new FormatException("Should be exaclty one correct item for non-multiple choice. BlockId=" + Id);
		}

		public override Component ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			var items = Items.Select(x => new Choice {Correct = x.IsCorrect, Text = x.Description}).ToArray();
			ChoiceResponse cr;
			if (Multiple)
			{
				var cg = new CheckboxGroup { Label = Text, Direction = "vertical", Choices = items };
				cr = new ChoiceResponse { ChoiceGroup = cg };
			}
			else
			{
				var cg = new MultipleChoiceGroup { Label = Text, Type = "MultipleChoice", Choices = items };
				cr = new MultipleChoiceResponse { ChoiceGroup = cg };
			}
			return new MultipleChoiceComponent { FolderName = folderName, UrlName = slide.Guid + componentIndex, ChoiceResponse = cr, Title = Text };
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

		public override Component ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			var items = new []
			{
				new Choice { Correct = Answer, Text = "true" },
				new Choice { Correct = !Answer, Text = "false" }
			};
			var cg = new MultipleChoiceGroup { Label = Text, Type = "MultipleChoice", Choices = items };
			return new MultipleChoiceComponent {FolderName = folderName, UrlName = slide.Guid + componentIndex, ChoiceResponse = new MultipleChoiceResponse {ChoiceGroup = cg}, Title = Text, Solution = new Solution(Explanation) };
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

		public override Component ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			return new TextInputComponent { FolderName = folderName, UrlName = slide.Guid + componentIndex, Title = Text, StringResponse = new StringResponse { Type = "ci", Answer = Regexes[0].Pattern, AdditionalAnswers = Regexes.Skip(1).Select(x => new Answer { Text = x.Pattern }).ToArray(), Textline = new Textline { Label = Text, Size = 20 } } };
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
