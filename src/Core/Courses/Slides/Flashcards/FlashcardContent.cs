using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Slides.Flashcards
{

	[XmlType(TypeName = "FlashcardInternals", Namespace = "https://ulearn.me/schema/v2")]
	public class FlashcardContent
	{
		[XmlElement("markdown", typeof(MarkdownBlock))]
		[XmlElement(typeof(CodeBlock))]
		[XmlElement(typeof(TexBlock))]
		[XmlElement(typeof(IncludeCodeBlock))]
		[XmlElement(typeof(IncludeMarkdownBlock))]
		[XmlElement("html", typeof(HtmlBlock))]
		public SlideBlock[] Blocks { get; set; }
	}
}