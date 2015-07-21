using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("course")]
	public class CourseWithChapters
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlAttribute("advanced_modules")]
		public string AdvancedModules;

		[XmlAttribute("lti_passports")]
		public string LtiPassports;

		[XmlAttribute("use_latex_compiler")]
		public bool UseLatexCompiler;

		[XmlElement("chapter")]
		public ChapterReference[] Chapters;
	}
}
