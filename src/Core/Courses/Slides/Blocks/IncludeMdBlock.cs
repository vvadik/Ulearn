using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;
using Ulearn.Common.Extensions;

namespace uLearn.Courses.Slides.Blocks
{
	[XmlType("include-md")]
	public class IncludeMdBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		public IncludeMdBlock(string file)
		{
			File = file;
		}

		public IncludeMdBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			yield return new MdBlock(context.Dir.GetContent(File)) { Hide = Hide };
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotSupportedException();
		}
	}
}