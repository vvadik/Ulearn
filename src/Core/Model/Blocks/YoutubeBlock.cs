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
			return $"Video {GetYoutubeUrl()}";
		}

		public VideoComponent GetVideoComponent(string displayName, Slide slide, int componentIndex, Dictionary<string, string> videoGuids)
		{
			if (videoGuids.ContainsKey(VideoId))
				return new VideoComponent(videoGuids[VideoId], displayName, VideoId);
			return new VideoComponent(slide.NormalizedGuid + componentIndex, displayName, VideoId);
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			throw new NotSupportedException();
		}

		public string GetYoutubeUrl()
		{
			return $"https://youtube.com/watch?v={VideoId}";
		}
	}
}