using System.Xml.Serialization;

namespace uLearn.Courses.Slides.Blocks
{
	///<summary>Отметка в коде</summary>
	public class Label
	{
		[XmlText]
		public string Name { get; set; }

		[XmlAttribute("only-body")]
		public bool OnlyBody { get; set; }
	}
}