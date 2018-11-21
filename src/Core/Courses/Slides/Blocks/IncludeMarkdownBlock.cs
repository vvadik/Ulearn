using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("include-markdown")]
	public class IncludeMarkdownBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		public IncludeMarkdownBlock(string file)
		{
			File = file;
		}

		public IncludeMarkdownBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideLoadingContext context, IImmutableSet<string> filesInProgress)
		{
			yield return new MarkdownBlock(context.UnitDirectory.GetContent(File)) { Hide = Hide };
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotSupportedException();
		}
	}
}