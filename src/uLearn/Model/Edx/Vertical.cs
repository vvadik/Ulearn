using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	[XmlRoot("vertical")]
	public class Vertical
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlIgnore]
		public Component[] Components;

		[XmlElement("video", Type = typeof(VideoComponentReference))]
		[XmlElement("html", Type = typeof(HtmlComponentReference))]
		[XmlElement("problem", Type = typeof(ProblemComponentReference))]
		[XmlElement("lti", Type = typeof(LtiComponentReference))]
		public EdxReference[] ComponentReferences
		{
			get { return Components.Select(x => x.GetReference()).ToArray(); }
		}
	}
}
