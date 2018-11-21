using System.Xml.Serialization;

namespace Ulearn.Core.Courses
{
	public class CourseLanguage
	{
		public CourseLanguage()
		{
		}

		public CourseLanguage(string name, string version)
		{
			Name = name;
			Version = version;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("version")]
		public string Version { get; set; }
	}
}