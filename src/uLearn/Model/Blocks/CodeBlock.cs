using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;
using uLearn.CSharp;

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
		public string Lang { get; set; }
		[XmlAttribute("lang-ver")]
		public string Version { get; set; }

		public CodeBlock(string code, string lang, string version = null)
		{
			Code = code;
			Lang = lang;
			Version = version;
		}

		public CodeBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			if (Version == null)
				Version = settings.GetLanguageVersion(Lang);
			yield return this;
		}

		public override string ToString()
		{
			return string.Format("{0} code {1}", Lang, Code);
		}
	}
}