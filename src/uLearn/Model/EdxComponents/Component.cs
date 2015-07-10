using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MarkdownDeep;

namespace uLearn.Model.EdxComponents
{
	public abstract class Component
	{
		[XmlIgnore]
		public string UrlName;

		[XmlIgnore]
		public string FolderName;

		public abstract void Save();
		public abstract ComponentReference GetReference();
	}

	[XmlRoot("video")]
	public class VideoComponent : Component
	{
		[XmlAttribute("url_name")]
		public new string UrlName;

		[XmlAttribute("youtube")]
		public string VideoId;

		[XmlAttribute("youtube_id_1_0")]
		public string VideoId1;

		[XmlAttribute("display_name")]
		public string DisplayName;

		public VideoComponent()
		{
		}

		public VideoComponent(string folderName, string urlName, string videoId)
		{
			FolderName = folderName;
			UrlName = urlName;
			VideoId = "1:00:" + videoId;
			VideoId1 = videoId;
		}

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/video/{1}.xml", FolderName, UrlName), this.Serialize());
		}

		public override ComponentReference GetReference()
		{
			return new VideoComponentReference { UrlName = UrlName };
		}
	}

	[XmlRoot("html")]
	public class HtmlComponent : Component
	{
		[XmlIgnore]
		public string Source;

		[XmlAttribute("filename")]
		public string Filename;

		[XmlAttribute("display_name")]
		public string DisplayName;

		public HtmlComponent()
		{
		}

		public HtmlComponent(string folderName, string urlName, string filename, string source)
		{
			FolderName = folderName;
			UrlName = urlName;
			Filename = filename;
			Source = source;
		}

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/html/{1}.xml", FolderName, UrlName), this.Serialize());
			File.WriteAllText(string.Format("{0}/html/{1}.html", FolderName, UrlName), Source);
		}

		public override ComponentReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}
	}

	[XmlRoot("html")]
	public class CodeComponent : Component
	{
		[XmlIgnore]
		public string Source;

		[XmlAttribute("filename")]
		public string Filename;

		[XmlAttribute("display_name")]
		public string DisplayName;

		public CodeComponent()
		{
		}

		public CodeComponent(string folderName, string urlName, string filename, string source)
		{
			FolderName = folderName;
			UrlName = urlName;
			Filename = filename;
			Source = source;
		}

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/html/{1}.xml", FolderName, UrlName), this.Serialize());
			File.WriteAllText(string.Format("{0}/html/{1}.html", FolderName, UrlName), File.ReadAllText("iframe-template.html").Replace("{0}", "code_" + UrlName));
			File.WriteAllText(string.Format("{0}/static/code_{1}.html", FolderName, UrlName), File.ReadAllText("code-template.html").Replace("{0}", Source));
		}

		public override ComponentReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}
	}

	public class ProblemComponent : Component
	{
		public override void Save()
		{
		}

		public override ComponentReference GetReference()
		{
			return new ProblemComponentReference { UrlName = UrlName };
		}
	}

	[XmlRoot("lti")]
	public class LtiComponent : Component
	{
		[XmlIgnore]
		public int SlideIndex;

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

		public LtiComponent()
		{
		}

		public LtiComponent(string folderName, string urlName, string launchUrl, string ltiId, bool hasScore, double weight, bool openInNewPage)
		{
			FolderName = folderName;
			UrlName = urlName;
			LaunchUrl = launchUrl;
			LtiId = ltiId;
			HasScore = hasScore;
			Weight = weight;
			OpenInNewPage = openInNewPage;
		}

		public override void Save()
		{
			File.WriteAllText(string.Format("{0}/lti/{1}.xml", FolderName, UrlName), this.Serialize());
		}

		public override ComponentReference GetReference()
		{
			return new LtiComponentReference { UrlName = UrlName };
		}
	}
}
