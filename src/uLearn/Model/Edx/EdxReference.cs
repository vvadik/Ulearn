using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	public class EdxReference
	{
		[XmlAttribute("url_name")]
		public string UrlName;

		public virtual Component GetComponent(string folderName, string urlName)
		{
			return null;
		}
	}

	public class ChapterReference : EdxReference
	{
	}

	public class SequentialReference : EdxReference
	{
	}

	public class VerticalReference : EdxReference
	{
	}

	public class HtmlComponentReference : EdxReference
	{
		public override Component GetComponent(string folderName, string urlName)
		{
			return HtmlComponent.Load(folderName, urlName);
		}
	}

	public class LtiComponentReference : EdxReference
	{
		public override Component GetComponent(string folderName, string urlName)
		{
			return LtiComponent.Load(folderName, urlName);
		}
	}

	public class VideoComponentReference : EdxReference
	{
		public override Component GetComponent(string folderName, string urlName)
		{
			return VideoComponent.Load(folderName, urlName);
		}
	}

	public class ProblemComponentReference : EdxReference
	{
		public override Component GetComponent(string folderName, string urlName)
		{
			return SlideProblemComponent.Load(folderName, urlName);
		}
	}
}
