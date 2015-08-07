using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace uLearn.Model.Edx
{
	[XmlRoot("course")]
	public class CourseWithChapters : EdxItem
	{
		[XmlIgnore]
		public override string SubfolderName { get { return "course"; } }

		[XmlAttribute("advanced_modules")]
		public string AdvancedModules;

		[XmlAttribute("lti_passports")]
		public string LtiPassports;

		[XmlAttribute("use_latex_compiler")]
		public bool UseLatexCompiler;

		[XmlElement("chapter")]
		public ChapterReference[] ChapterReferences { get; set; }

		[XmlIgnore]
		public Chapter[] Chapters;

		public CourseWithChapters()
		{
		}

		public CourseWithChapters(string urlName, string displayName, string[] advancedModules, string[] ltiPassports, bool useLatexCompiler, Chapter[] chapters)
		{
			UrlName = urlName;
			DisplayName = displayName;
			AdvancedModules = advancedModules == null ? null : JsonConvert.SerializeObject(advancedModules);
			LtiPassports = ltiPassports == null ? null : JsonConvert.SerializeObject(ltiPassports);
			UseLatexCompiler = useLatexCompiler;
			Chapters = chapters;
			ChapterReferences = chapters.Select(x => x.GetReference()).ToArray();
		}

		public override void SaveAdditional(string folderName)
		{
			foreach (var chapter in Chapters)
				chapter.Save(folderName);
		}

		public static CourseWithChapters Load(string folderName, string urlName)
		{
			var courseWithChapters = new FileInfo(string.Format("{0}/course/{1}.xml", folderName, urlName)).DeserializeXml<CourseWithChapters>();
			courseWithChapters.UrlName = urlName;
			courseWithChapters.Chapters = courseWithChapters.ChapterReferences.Select(x => Chapter.Load(folderName, x.UrlName)).ToArray();
			return courseWithChapters;
		}
	}
}
