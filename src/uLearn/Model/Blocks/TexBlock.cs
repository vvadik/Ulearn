using System;
using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("tex")]
	public class TexBlock : SlideBlock
	{
		[XmlElement("line")]
		public string[] TexLines { get; set; }

		public TexBlock(string[] texLines)
		{
			TexLines = texLines;
		}

		public TexBlock()
		{
		}

		public override string ToString()
		{
			return string.Format("Tex {0}", string.Join("\n", TexLines));
		}

		public override Component ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			var urlName = slide.Guid + componentIndex;
			return new HtmlComponent(folderName, urlName, urlName, string.Join("\n", TexLines.Select(x => "$$" + x + "$$")).GetHtml("static"));
		}
	}
}