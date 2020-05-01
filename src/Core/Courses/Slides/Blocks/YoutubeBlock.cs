using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("youtube")]
	[DataContract]
	public class YoutubeBlock : SlideBlock, IApiSlideBlock
	{
		[XmlText]
		[DataMember(Name = "videoId")]
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

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotSupportedException();
		}

		public string GetYoutubeUrl()
		{
			return $"https://youtube.com/watch?v={VideoId}";
		}

		[XmlIgnore]
		[DataMember(Name = "type")]
		public string Type { get; set; } = "youtube";
	}
}