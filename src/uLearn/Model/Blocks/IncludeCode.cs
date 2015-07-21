using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using uLearn.Model.Edx.EdxComponents;

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


		protected void FillProperties(BuildUpContext context)
		{
			File = File ?? context.Lesson.DefaultIncludeFile;
			LangId = LangId ?? (Path.GetExtension(File) ?? "").Trim('.');
			LangVer = LangVer ?? context.CourseSettings.GetLanguageVersion(LangId);
		}

		public override IEnumerable<Component> ToEdxComponent(string folderName, string courseId, string displayName, Slide slide, int componentIndex)
		{
			if (!string.IsNullOrEmpty(File))
				throw new Exception("IncludeCode.cs: File string is not empty.");
			return new [] { new CodeComponent() };
		}
	}
}