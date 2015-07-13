using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using MarkdownDeep;

namespace uLearn.Model.EdxComponents
{
	public abstract class Component
	{
		[XmlIgnore]
		public virtual string UrlName { get; set; }

		[XmlIgnore]
		public string FolderName;

		[XmlIgnore]
		public virtual string SubfolderName { get; set; }

		public virtual void Save()
		{
			File.WriteAllText(string.Format("{0}/{1}/{2}.xml", FolderName, SubfolderName, UrlName), this.Serialize());
		}

		public abstract ComponentReference GetReference();
		public abstract XmlElement AsXmlElement();
	}

	[XmlRoot("video")]
	public class VideoComponent : Component
	{
		[XmlAttribute("url_name")]
		public override string UrlName { get; set; }

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
			set { }
		}

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

		public override ComponentReference GetReference()
		{
			return new VideoComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			throw new NotImplementedException();
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

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "html"; }
			set { }
		}

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
			File.WriteAllText(string.Format("{0}/{1}/{2}.xml", FolderName, SubfolderName, UrlName), this.Serialize());
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", FolderName, SubfolderName, UrlName), Source);
		}

		public override ComponentReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			var doc = new XmlDocument();
			doc.LoadXml("<p>" + Source + "</p>");
			return doc.DocumentElement;
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

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "html"; }
			set { }
		}

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
			File.WriteAllText(string.Format("{0}/{1}/{2}.xml", FolderName, SubfolderName, UrlName), this.Serialize());
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", FolderName, SubfolderName, UrlName), File.ReadAllText("iframe-template.html").Replace("{0}", "code_" + UrlName));
			File.WriteAllText(string.Format("{0}/static/code_{1}.html", FolderName, UrlName), File.ReadAllText("code-template.html").Replace("{0}", Source));
		}

		public override ComponentReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			var doc = new XmlDocument();
			doc.LoadXml(File.ReadAllText("iframe-template.html").Replace("{0}", "code_" + UrlName));
			return doc.DocumentElement;
		}
	}

	[XmlRoot("problem")]
	public class SlideProblemComponent : Component
	{
		[XmlElement("p")]
		public XmlElement[] Components;

		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "problem"; }
			set { }
		}

		public override ComponentReference GetReference()
		{
			return new ProblemComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			throw new NotImplementedException();
		}
	}

	public class ProblemComponent : Component
	{
		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "problem"; }
			set { }
		}

		public override ComponentReference GetReference()
		{
			return new ProblemComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			var xmlString = this.Serialize();
			xmlString = xmlString.Substring("<problem>".Length, xmlString.Length - "<problem></problem>".Length);
			var doc = new XmlDocument();
			doc.LoadXml("<p>" + xmlString + "</p>");
			return doc.DocumentElement;
		}
	}

	[XmlRoot("lti")]
	public class LtiComponent : Component
	{
		[XmlIgnore]
		public int SlideIndex;

		[XmlAttribute("display_name")]
		public string DisplayName;

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
			set { }
		}

		public LtiComponent()
		{
		}

		public LtiComponent(string displayName, string folderName, string urlName, string launchUrl, string ltiId, bool hasScore, double weight, bool openInNewPage)
		{
			DisplayName = displayName;
			FolderName = folderName;
			UrlName = urlName;
			LaunchUrl = launchUrl;
			LtiId = ltiId;
			HasScore = hasScore;
			Weight = weight;
			OpenInNewPage = openInNewPage;
		}

		public override ComponentReference GetReference()
		{
			return new LtiComponentReference { UrlName = UrlName };
		}

		public override XmlElement AsXmlElement()
		{
			throw new NotImplementedException();
		}
	}
}
