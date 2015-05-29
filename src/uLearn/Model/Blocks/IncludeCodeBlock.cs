using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;
using uLearn.CSharp;

namespace uLearn.Model.Blocks
{
	[XmlType("include-code")]
	public class IncludeCodeBlock : IncludeCode
	{
		[XmlElement("display")]
		public Label[] DisplayLabels { get; set; }

		public IncludeCodeBlock(string file) : base(file)
		{
			File = file;
		}

		public IncludeCodeBlock()
		{
		}


		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			FillProperties(settings, lesson);
			DisplayLabels = DisplayLabels ?? new Label[0];
			var content = fs.GetContent(File);

			if (DisplayLabels.Length == 0)
			{
				yield return new CodeBlock(content, LangId, LangVer);
				yield break;
			}

			var extractor = new RegionsExtractor(content, LangId);
			yield return new CodeBlock(String.Join("\r\n\r\n", extractor.GetRegions(DisplayLabels)), LangId, LangVer);
		}
	}
}