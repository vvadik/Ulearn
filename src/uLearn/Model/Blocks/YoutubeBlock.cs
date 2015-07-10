using System;
using System.Xml.Serialization;
using uLearn.Model.EdxComponents;

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

		public override Component ToEdxComponent(string folderName, string courseId, Slide slide, int componentIndex)
		{
			return new VideoComponent(folderName, slide.Guid + componentIndex, VideoId);
		}
	}
}