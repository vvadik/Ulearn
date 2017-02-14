using System;
using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("lti")]
	public class LtiComponent : Component
	{
		[XmlAttribute("has_score")]
		public bool HasScore;

		[XmlAttribute("launch_url")]
		public string LaunchUrl;

		[XmlAttribute("lti_id")]
		public string LtiId;

		[XmlAttribute("open_in_a_new_page")]
		public bool OpenInNewPage;

		[XmlAttribute("weight")]
		public double Weight;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "lti"; }
		}

		public LtiComponent()
		{
		}

		public LtiComponent(string displayName, string urlName, string launchUrl, string ltiId, bool hasScore, double weight, bool openInNewPage)
		{
			DisplayName = displayName;
			UrlName = urlName;
			LaunchUrl = launchUrl;
			LtiId = ltiId;
			HasScore = hasScore;
			Weight = weight;
			OpenInNewPage = openInNewPage;
		}

		public override EdxReference GetReference()
		{
			return new LtiComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
            throw new NotSupportedException();
        }

		public static LtiComponent Load(string folderName, string urlName, EdxLoadOptions options)
		{
			return Load<LtiComponent>(folderName, "lti", urlName, options);
		}
	}
}
