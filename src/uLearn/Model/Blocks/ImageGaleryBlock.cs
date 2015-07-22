using System.Collections.Generic;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
	[XmlType("gallery-images")]
	public class ImageGaleryBlock : SlideBlock
	{
		[XmlElement("image")]
		public string[] ImageUrls { get; set; }

		public ImageGaleryBlock(string[] images)
		{
			ImageUrls = images;
		}

		public ImageGaleryBlock()
		{
		}

		public override string ToString()
		{
			return string.Format("Images {0}", string.Join("\n", ImageUrls));
		}

		public override Component ToEdxComponents(string displayName, Slide slide, int componentIndex)
		{
			var urlName = slide.Guid + componentIndex;
			return new GalleryComponent(urlName, displayName, urlName, slide.Info.SlideFile.Directory.FullName, ImageUrls);
		}
	}
}