using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class YoutubeBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[XmlText]
		[DataMember(Name = "videoId")]
		public string VideoId { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "youtube";

		[DataMember(Name = "annotation")]
		public Annotation Annotation { get; set; }

		[DataMember(Name = "googleDocLink")]
		public string GoogleDocLink { get; set; }

		public YoutubeBlockResponse(YoutubeBlock youtubeBlock, Annotation annotation, string googleDocLink)
		{
			Hide = youtubeBlock.Hide;
			VideoId = youtubeBlock.VideoId;
			Annotation = annotation;
			GoogleDocLink = googleDocLink;
		}
	}
}