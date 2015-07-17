using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;
using uLearn.Model.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("code")]
	public class CodeBlock : SlideBlock
	{
		private string code;

		[XmlText]
		public string Code
		{
			get { return code; }
			set { code = value.RemoveCommonNesting().TrimEnd(); }
		}

		[XmlAttribute("lang-id")]
		public string LangId { get; set; }
		[XmlAttribute("lang-ver")]
		public string LangVer { get; set; }

		public CodeBlock(string code, string langId, string langVer = null)
		{
			Code = code;
			LangId = langId;
			LangVer = langVer;
		}

		public CodeBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			if (LangVer == null)
				LangVer = context.CourseSettings.GetLanguageVersion(LangId);
			yield return this;
		}

		public override IEnumerable<Component> ToEdxComponent(string folderName, string courseId, string displayName, Slide slide, int componentIndex)
		{
			var urlName = slide.Guid + componentIndex;
			return new [] { new CodeComponent(folderName, urlName, displayName, urlName, LangId, Code) };
		}

		public override string ToString()
		{
			return string.Format("{0} code {1}", LangId, Code);
		}
	}
}