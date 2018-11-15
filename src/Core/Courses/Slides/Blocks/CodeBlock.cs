using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("code")]
	public class CodeBlock : SlideBlock
	{
		private string code;

		[XmlText]
		public string Code
		{
			get => code;
			set => code = value.RemoveCommonNesting().TrimEnd();
		}

		[XmlAttribute("lang-id")]
		public string LangId { get; set; }

		[XmlAttribute("lang-ver")]
		public string LangVer { get; set; }
		
		[XmlIgnore]
		public List<Label> SourceCodeLabels { get; set; } = new List<Label>();

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
			if (LangId == null)
				LangId = context.CourseSettings.DefaultLanguage;
			if (LangVer == null)
				LangVer = context.CourseSettings.GetLanguageVersion(LangId);
			yield return this;
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			var urlName = slide.NormalizedGuid + componentIndex;
			return new CodeComponent(urlName, displayName, urlName, LangId, Code);
		}

		public override string ToString()
		{
			return $"{LangId} code {Code}";
		}

		public override string TryGetText()
		{
			return Code;
		}
	}
}