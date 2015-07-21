using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("youtube")]
	public class YoutubeBlock : SlideBlock
	{
		[XmlText]
		public string VideoId { get; set; }

		public YoutubeBlock(string videoId)
		{
			VideoId = videoId;
		}

		public YoutubeBlock()
		{
		}

		public override string ToString()
		{
			return string.Format("Video {0}", VideoId);
		}

		public override IEnumerable<Component> ToEdxComponent(string folderName, string courseId, string displayName, Slide slide, int componentIndex)
		{
			return new [] { new VideoComponent(folderName, slide.Guid + componentIndex, displayName, VideoId) };
		}
	}
}