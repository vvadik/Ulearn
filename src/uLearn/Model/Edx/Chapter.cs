using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("chapter")]
	public class Chapter
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlElement("sequential")]
		public SequentialReference[] Sequentials;
	}
}
