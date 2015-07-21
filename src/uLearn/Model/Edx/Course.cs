using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
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

		public void Save()
		{
			File.WriteAllText(string.Format("{0}/course.xml", ""
				), this.XmlSerialize());
		}
	}
}
