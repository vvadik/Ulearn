using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.Model.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("md")]
	public class MdBlock : SlideBlock
	{
		private string markdown;

		[XmlText]
		public string Markdown
		{
			get { return markdown; }
			set { markdown = value.RemoveCommonNesting(); }
		}

		public MdBlock(string markdown)
		{
			if (markdown != null)
				Markdown = markdown.TrimEnd();
		}

		public MdBlock()
		{
		}

		public override string ToString()
		{
			return string.Format("Markdown {0}", Markdown);
		}

		public override IEnumerable<Component> ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			var urlName = slide.Guid + componentIndex;
			return new [] { new HtmlComponent(folderName, urlName, urlName, Markdown.GetHtml("static")) };
		}
	}
}