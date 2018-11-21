using System;
using System.Linq;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;

namespace Ulearn.Core.Courses.Slides.Quizzes
{
	/*
	[XmlRoot("Quiz", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/quiz")]	 
	public class Quiz
	{
		public Quiz InitQuestionIndices()
		{
			var index = 1;
			foreach (var b in Blocks.OfType<AbstractQuestionBlock>())
				b.QuestionIndex = index++;
			return this;
		}

		[XmlAttribute("id")]
		public string Id;
		
		[Obsolete("Use MaxTriesCount (attribute 'tries' of <scoring> tag in xml) instead")]
		[XmlAttribute("maxDropCount")]
		public int MaxDropCount;

		[XmlAttribute("tries")]
		public int MaxTriesCount;

		[XmlAttribute("manualCheck")]
		public bool ManualChecking;

		[XmlAttribute("scoringGroup")]
		public string ScoringGroup;

		[XmlElement("meta")]
		public SlideMetaDescription Meta { get; set; }
		
		[XmlElement("title")]
		public string Title;

		private SlideBlock[] blocks;

		[XmlElement("p", Type = typeof(MarkdownBlock))]
		[XmlElement(typeof(CodeBlock))]
		[XmlElement(typeof(TexBlock))]
		[XmlElement(typeof(IncludeCodeBlock))]
		[XmlElement("isTrue", Type = typeof(IsTrueBlock))]
		[XmlElement("choice", Type = typeof(ChoiceBlock))]
		[XmlElement("fillIn", Type = typeof(FillInBlock))]
		[XmlElement("ordering", Type = typeof(OrderingBlock))]
		[XmlElement("matching", Type = typeof(MatchingBlock))]
		public SlideBlock[] Blocks
		{
			get => blocks ?? new SlideBlock[0];
			set => blocks = value;
		}

		public string NormalizedXml => this.XmlSerialize(true);

		public int MaxScore
		{
			get { return Blocks.OfType<AbstractQuestionBlock>().Sum(b => b.MaxScore); }
		}

		public bool HasEqualStructureWith(Quiz other)
		{
			if (Blocks.Length != other.Blocks.Length)
				return false;
			for (var blockIdx = 0; blockIdx < Blocks.Length; blockIdx++)
			{
				var block = Blocks[blockIdx];
				var otherBlock = other.Blocks[blockIdx];
				var questionBlock = block as AbstractQuestionBlock;
				var otherQuestionBlock = otherBlock as AbstractQuestionBlock;
				/* Ignore non-question blocks #1#
				if (questionBlock == null)
					continue;
				/* If our block is question, block in other slide must be question with the same Id #1#
				if (otherQuestionBlock == null || questionBlock.Id != otherQuestionBlock.Id)
					return false;

				if (!questionBlock.HasEqualStructureWith(otherQuestionBlock))
					return false;
			}
			return true;
		}
	}
	*/
}