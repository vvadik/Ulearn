using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("tex")]
	[DataContract]
	public class TexBlock : SlideBlock, IApiSlideBlock
	{
		[XmlElement("line")]
		[DataMember(Name = "lines")]
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

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
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

		[XmlIgnore]
		[DataMember(Name = "type")]
		public string Type { get; set; } = "tex";
	}
}