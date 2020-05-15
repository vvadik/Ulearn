using System.ComponentModel;
using System.Runtime.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class HtmlBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[DataMember(Name = "content")]
		public string Content { get; set; }

		[DataMember(Name = "fromMarkdown")]
		public bool FromMarkdown { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "html";

		public HtmlBlockResponse(HtmlBlock htmlBlock, bool fromMarkdown)
		{
			Hide = htmlBlock.Hide;
			Content = htmlBlock.Content;
			FromMarkdown = fromMarkdown;
		}

		public HtmlBlockResponse()
		{
		}
	}
}