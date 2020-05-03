using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("galleryImages")]
	public class ImageGalleryBlock : SlideBlock, IApiSlideBlock
	{
		[XmlElement("image")]
		[DataMember(Name = "imageUrls")]
		public string[] ImageUrls { get; set; }

		public ImageGalleryBlock(string[] images)
		{
			ImageUrls = images;
		}

		public ImageGalleryBlock()
		{
		}

		public override string ToString()
		{
			return $"Gallery with images:\n{string.Join("\n", ImageUrls)}";
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			var urlName = slide.NormalizedGuid + componentIndex;
			return new GalleryComponent(urlName, displayName, urlName, slide.Info.SlideFile.Directory.FullName, ImageUrls);
		}

		[XmlIgnore]
		[DataMember(Name = "type")]
		public string Type { get; set; } = "imageGallery";
	}
}