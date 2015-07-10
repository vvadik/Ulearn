using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearnToEdx.Edx
{
	[XmlRoot("chapter")]
	public class Chapter
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlElement("sequential")]
		public SequentialUrl[] Sequentials;
	}

	public class SequentialUrl
	{
		[XmlAttribute("url_name")]
		public string UrlName;
	}
}
