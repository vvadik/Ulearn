using System.Xml.Serialization;

namespace uLearnToEdx.Edx
{
	[XmlRoot("course")]
	public class Course
	{
		[XmlAttribute("url_name")]
		public string UrlName;

		[XmlAttribute("org")]
		public string Organization;

		[XmlAttribute("course")]
		public string CourseName;
	}
}
