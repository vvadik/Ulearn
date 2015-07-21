using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	public class EdxReference
	{
		[XmlAttribute("url_name")]
		public string UrlName;
	}

	public class ChapterReference : EdxReference { }
	public class SequentialReference : EdxReference { }
	public class VerticalReference : EdxReference { }

	public class HtmlComponentReference : EdxReference { }
	public class LtiComponentReference : EdxReference { }
	public class VideoComponentReference : EdxReference { }
	public class ProblemComponentReference : EdxReference { }
}
