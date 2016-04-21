using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using uLearn.Model.Blocks;
using uLearn.Model.Edx.EdxComponents;

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
		[XmlElement("ordering", Type = typeof(OrderingBlock))]
		[XmlElement("matching", Type = typeof(MatchingBlock))]
		public SlideBlock[] Blocks
		{
			get { return blocks ?? new SlideBlock[0]; }
			set { blocks = value; }
		}

		public string NormalizedXml
		{
			get { return this.XmlSerialize(true); }
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

		public override string TryGetText()
		{
			return Text;
		}
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
			if (Items.DistinctBy(i => i.Id).Count() != Items.Length)
				throw new FormatException("Duplicate choice id in quizBlock " + Id);
			if (!Multiple && Items.Count(i => i.IsCorrect) != 1)
				throw new FormatException("Should be exaclty one correct item for non-multiple choice. BlockId=" + Id);
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			var items = Items.Select(x => new Choice { Correct = x.IsCorrect, Text = EdxTexReplacer.ReplaceTex(x.Description) }).ToArray();
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
			return new MultipleChoiceComponent
			{
				UrlName = slide.NormalizedGuid + componentIndex,
				ChoiceResponse = cr,
				Title = EdxTexReplacer.ReplaceTex(Text)
			};
		}

		public override string TryGetText()
		{
			return Text + '\n' + string.Join("\n", Items.Select(item => item.GetText()));
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

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			var items = new []
			{
				new Choice { Correct = Answer, Text = "true" },
				new Choice { Correct = !Answer, Text = "false" }
			};
			var cg = new MultipleChoiceGroup { Label = Text, Type = "MultipleChoice", Choices = items };
			return new MultipleChoiceComponent
			{
				UrlName = slide.NormalizedGuid + componentIndex,
				ChoiceResponse = new MultipleChoiceResponse { ChoiceGroup = cg },
				Title = EdxTexReplacer.ReplaceTex(Text),
				Solution = new Solution(Explanation)
			};
		}

		public override string TryGetText()
		{
			return Text + '\n' + Explanation;
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

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			return new TextInputComponent
			{
				UrlName = slide.NormalizedGuid + componentIndex,
				Title = EdxTexReplacer.ReplaceTex(Text),
				StringResponse = new StringResponse
				{
					Type = (Regexes[0].IgnoreCase ? "ci " : "") + "regexp",
					Answer = "^" + Regexes[0].Pattern + "$",
					AdditionalAnswers = Regexes.Skip(1).Select(x => new Answer { Text = "^" + x.Pattern + "$" }).ToArray(),
					Textline = new Textline { Label = Text, Size = 20 }
				}
			};
		}

		public override string TryGetText()
		{
			return Text + '\n' + Sample + '\t' + Explanation;
		}
	}

	public class OrderingBlock : AbstractQuestionBlock
	{
		[XmlElement("item")]
		public OrderingItem[] Items;

		[XmlElement("explanation")]
		public string Explanation;

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			throw new NotImplementedException();
		}

		public OrderingItem[] ShuffledItems()
		{
			return Items.Shuffle().ToArray();
		}
	}

	public class MatchingBlock : AbstractQuestionBlock
	{
		[XmlAttribute("shuffleFixed")]
		public bool ShuffleFixed;

		[XmlElement("explanation")]
		public string Explanation;

		[XmlElement("match")]
		public MatchingMatch[] Matches;

		private readonly Random random = new Random();

		public List<MatchingMatch> GetMatches(bool shuffle=false)
		{
			if (shuffle)
				return Matches.Shuffle(random).ToList();

			return Matches.ToList();
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			throw new NotImplementedException();
		}
	}

	public class OrderingItem
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlText]
		public string Text;

		public string GetHash()
		{
			return (Id + "OrderingItemSalt").GetHashCode().ToString();
		}
	}

	public class MatchingMatch
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("fixed")]
		public string FixedItem;

		[XmlElement("movable")]
		public string MovableItem;

		public string GetHashForFixedItem()
		{
			return (Id + "MatchingItemFixedItemSalt").GetHashCode().ToString();
		}

		public string GetHashForMovableItem()
		{
			return (Id + "MatchingItemMovableItemSalt").GetHashCode().ToString();
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

		public string GetText()
		{
			return (Description ?? "") + '\t' + (Explanation ?? "");
		}
	}
}
