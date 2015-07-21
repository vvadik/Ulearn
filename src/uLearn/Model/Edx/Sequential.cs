using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("sequential")]
	public class Sequential
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlElement("vertical")]
		public VerticalReference[] Verticals;
	}
}
