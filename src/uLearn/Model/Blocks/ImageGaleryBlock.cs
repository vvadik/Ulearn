using System.Xml.Serialization;

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
	}
}