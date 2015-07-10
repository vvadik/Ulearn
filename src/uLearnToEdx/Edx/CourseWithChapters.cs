using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearnToEdx.Edx
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
		public ChapterUrl[] Chapters;
	}

	public class ChapterUrl
	{
		[XmlAttribute("url_name")]
		public string UrlName;
	}
}
