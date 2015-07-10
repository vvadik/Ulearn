using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearn.Model.EdxComponents
{
	public class ComponentReference
	{
		[XmlAttribute("url_name")]
		public string UrlName;
	}

	public class HtmlComponentReference : ComponentReference { }
	public class LtiComponentReference : ComponentReference { }
	public class VideoComponentReference : ComponentReference { }
	public class ProblemComponentReference : ComponentReference { }
}
