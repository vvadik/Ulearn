using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using uLearn.Model.EdxComponents;

namespace uLearnToEdx.Edx
{
	[XmlRoot("vertical")]
	public class Vertical
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlElement("video", Type = typeof(VideoComponentReference))]
		[XmlElement("html", Type = typeof(HtmlComponentReference))]
		[XmlElement("problem", Type = typeof(ProblemComponentReference))]
		[XmlElement("lti", Type = typeof(LtiComponentReference))]
		public ComponentReference[] Components;
	}


}
