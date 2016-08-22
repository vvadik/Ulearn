using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("include-blocks")]
	public class IncludeBlocksBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		public IncludeBlocksBlock(string file)
		{
			File = file;
		}

		public IncludeBlocksBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			if (filesInProgress.Contains(File))
				throw new Exception("Cyclic dependency");

			var xmlStream = new StringReader(context.Dir.GetContent(File));
			var serializer = new XmlSerializer(typeof(SlideBlock[]));
			var slideBlocks = (SlideBlock[])serializer.Deserialize(xmlStream);
			var newInProgress = filesInProgress.Add(File);
			return slideBlocks.SelectMany(b => b.BuildUp(context, newInProgress));
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			throw new NotSupportedException();
		}
	}
}