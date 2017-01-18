using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

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
			return $"Tex {string.Join("\n", TexLines)}";
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			var urlName = slide.NormalizedGuid + componentIndex;
			return new HtmlComponent(
				urlName,
				displayName,
				urlName,
				string.Join("\n", TexLines.Select(x => "$$" + x + "$$")).GetHtmlWithUrls("/static").Item1
				);
		}

		public override string TryGetText()
		{
			return string.Join("\n", TexLines);
		}
	}
}