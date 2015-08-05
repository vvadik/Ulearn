using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("video")]
	public class VideoComponent : Component
	{
		[XmlAttribute("youtube")]
		public string VideoId;

		[XmlAttribute("youtube_id_1_0")]
		public string NormalSpeedVideoId;

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
			NormalSpeedVideoId = videoId;
		}

		public override EdxReference GetReference()
		{
			return new VideoComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return string.Format(@"<iframe class=""embedded-video"" width=""100%"" height=""530"" src=""//www.youtube.com/embed/{0}"" frameborder=""0"" allowfullscreen=""\""></iframe>", NormalSpeedVideoId);
		}

		public static VideoComponent Load(string folderName, string urlName)
		{
			var component = new FileInfo(string.Format("{0}/video/{1}.xml", folderName, urlName)).DeserializeXml<VideoComponent>();
			component.UrlName = urlName;
			return component;
		}
	}
}
