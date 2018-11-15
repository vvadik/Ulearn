using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("include-code")]
	public class IncludeCodeBlock : IncludeCode
	{
		public IncludeCodeBlock(string codeFile)
			: base(codeFile)
		{
			CodeFile = codeFile;
		}

		public IncludeCodeBlock()
		{
		}

		[XmlElement("display")]
		public Label[] DisplayLabels { get; set; }


		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			DisplayLabels = DisplayLabels ?? new Label[0];

			if (DisplayLabels.Length == 0)
			{
				var content = context.Dir.GetContent(CodeFile);
				yield return new CodeBlock(content, LangId, LangVer) { Hide = Hide };
				yield break;
			}

			var extractor = context.GetExtractor(CodeFile, LangId);
			yield return new CodeBlock(string.Join("\r\n\r\n", extractor.GetRegions(DisplayLabels, withoutAttributes: true)), LangId, LangVer) { Hide = Hide };
		}
	}
}