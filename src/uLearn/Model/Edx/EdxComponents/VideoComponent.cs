using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("video")]
	public class VideoComponent : Component
	{
		[XmlAttribute("youtube")]
		public string VideoId;

		[XmlAttribute("youtube_id_1_0")]
		public string VideoId1;

		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "video"; }
		}

		public VideoComponent()
		{
		}

		public VideoComponent(string urlName, string displayName, string videoId)
		{
			UrlName = urlName;
			DisplayName = displayName;
			VideoId = "1:00:" + videoId;
			VideoId1 = videoId;
		}

		public override EdxReference GetReference()
		{
			return new VideoComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return "<iframe class=\"embedded-video\" width=\"100%\" height=\"530\" src=\"//www.youtube.com/embed/1WaWDgBxyYc\" frameborder=\"0\" allowfullscreen=\"\"></iframe>";
		}
	}
}
