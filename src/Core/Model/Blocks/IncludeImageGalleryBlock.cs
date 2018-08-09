using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;
using Ulearn.Common.Extensions;

namespace uLearn.Model.Blocks
{
	[XmlType("gallery")]
	public class IncludeImageGalleryBlock : SlideBlock
	{
		[XmlText]
		public string Directory { get; set; }

		public IncludeImageGalleryBlock(string directory)
		{
			Directory = directory;
		}

		public IncludeImageGalleryBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			yield return new ImageGaleryBlock(context.Dir.GetFilenames(Directory)) { Hide = Hide };
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotSupportedException();
		}
	}
}