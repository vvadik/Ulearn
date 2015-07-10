using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearnToEdx.Edx
{
	[XmlRoot("sequential")]
	public class Sequential
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlElement("vertical")]
		public VerticalUrl[] Verticals;
	}

	public class VerticalUrl
	{
		[XmlAttribute("url_name")]
		public string UrlName;
	}
}
