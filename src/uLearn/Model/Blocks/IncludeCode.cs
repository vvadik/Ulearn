using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Blocks
{
	public class IncludeCode : SlideBlock
	{
		protected IncludeCode(string file)
		{
			File = file;
		}

		public IncludeCode()
		{
		}

		[XmlAttribute("file")]
		public string File { get; set; }

		[XmlAttribute("lang-id")]
		public string LangId { get; set; }

		[XmlAttribute("lang-ver")]
		public string LangVer { get; set; }


		protected void FillProperties(CourseSettings settings, Lesson lesson)
		{
			File = File ?? lesson.DefaultIncludeFile;
			LangId = LangId ?? (Path.GetExtension(File) ?? "").Trim('.');
			LangVer = LangVer ?? settings.GetLanguageVersion(LangId);
		}
	}
}