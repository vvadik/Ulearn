using System.ComponentModel;
using System.Runtime.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class ImageGalleryBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[DataMember(Name = "imageUrls")]
		public string[] ImageUrls { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "imageGallery";

		public ImageGalleryBlockResponse(ImageGalleryBlock texBlock)
		{
			Hide = texBlock.Hide;
			ImageUrls = texBlock.ImageUrls;
		}

		public ImageGalleryBlockResponse()
		{
		}
	}
}