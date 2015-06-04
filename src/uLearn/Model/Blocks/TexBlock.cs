using System.Xml.Serialization;

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
	}
}