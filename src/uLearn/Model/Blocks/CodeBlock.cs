using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;

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

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			if (LangVer == null)
				LangVer = settings.GetLanguageVersion(LangId);
			yield return this;
		}

		public override string ToString()
		{
			return string.Format("{0} code {1}", LangId, Code);
		}
	}
}