using System;
using System.IO;
using System.Xml.Serialization;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	public class IncludeCode : SlideBlock
	{
		protected IncludeCode(string codeFile)
		{
			CodeFile = codeFile;
		}

		public IncludeCode()
		{
		}

		[XmlAttribute("file")]
		public string CodeFile { get; set; }

		[XmlAttribute("lang-id")]
		public string LangId { get; set; }

		[XmlAttribute("lang-ver")]
		public string LangVer { get; set; }

		protected void FillProperties(BuildUpContext context)
		{
			CodeFile = CodeFile ?? context.Lesson?.DefaultIncludeCodeFile ?? context.Unit.Settings?.DefaultIncludeCodeFile;
			LangId = LangId ?? Path.GetExtension(CodeFile)?.Trim('.') ?? context.CourseSettings.DefaultLanguage;
			LangVer = LangVer ?? context.CourseSettings.GetLanguageVersion(LangId);
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			if (!string.IsNullOrEmpty(CodeFile))
				throw new Exception("IncludeCode.cs: File string is not empty.");
			return new CodeComponent();
		}
	}
}