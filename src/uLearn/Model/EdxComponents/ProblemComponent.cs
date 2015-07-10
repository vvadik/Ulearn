using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace uLearn.Model.EdxComponents
{
	[XmlRoot("problem")]
	public class TextInputComponent : ProblemComponent
	{
		[XmlElement("p")]
		public string Title;

		[XmlElement("stringresponse")]
		public StringResponse StringResponse;

		[XmlElement("solution")]
		public Solution Solution;

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/problem/{1}.xml", FolderName, UrlName), this.Serialize());
		}
	}

	public class StringResponse
	{
		[XmlAttribute("answer")]
		public string Answer;

		[XmlAttribute("type")]
		public string Type;

		[XmlElement("additional_answer")]
		public Answer[] AdditionalAnswers;

		[XmlElement("textline")]
		public Textline Textline;
	}

	public class Answer
	{
		[XmlText]
		public string Text;
	}

	public class Textline
	{
		[XmlAttribute("label")]
		public string Label;

		[XmlAttribute("size")]
		public int Size;
	}

	[XmlRoot("problem")]
	public class MultipleChoiceComponent : ProblemComponent
	{
		[XmlElement("p")]
		public string Title;

		[XmlElement("multiplechoiceresponse", Type = typeof(MultipleChoiceResponse))]
		[XmlElement("choiceresponse", Type = typeof(ChoiceResponse))]
		public ChoiceResponse ChoiceResponse;

		[XmlElement("solution")]
		public Solution Solution;

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/problem/{1}.xml", FolderName, UrlName), this.Serialize());
		}
	}

	public class Solution
	{
		[XmlText]
		public string Text;

		public Solution()
		{
		}

		public Solution(string text)
		{
//			Text = string.Format("<div class=\"detailed-solution\"><p>Explanation</p><p>{0}</p></div>", text);
		}
	}

	public class ChoiceResponse
	{
		[XmlElement("choicegroup", Type = typeof(MultipleChoiceGroup))]
		[XmlElement("checkboxgroup", Type = typeof(CheckboxGroup))]
		public ChoiceGroup ChoiceGroup;
	}

	public class MultipleChoiceResponse : ChoiceResponse
	{
		
	}

	public class ChoiceGroup
	{
		[XmlAttribute("label")]
		public string Label;

		[XmlElement("choice")]
		public Choice[] Choices;
	}


	public class MultipleChoiceGroup : ChoiceGroup
	{
		[XmlAttribute("type")]
		public string Type;
	}

	public class CheckboxGroup : ChoiceGroup
	{
		[XmlAttribute("direction")]
		public string Direction;
	}

	public class Choice
	{
		[XmlAttribute("correct")]
		public bool Correct;

		[XmlText]
		public string Text;
	}
}
