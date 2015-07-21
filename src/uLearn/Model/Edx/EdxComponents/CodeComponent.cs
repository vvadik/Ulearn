using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("html")]
	public class CodeComponent : Component
	{
		[XmlIgnore]
		public string Source;

		[XmlIgnore]
		public string LangId;

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

		public CodeComponent(string folderName, string urlName, string displayName, string filename, string langId, string source)
		{
			FolderName = folderName;
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			LangId = langId;
			Source = source;
		}

		public override void Save()
		{
			base.Save();
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", FolderName, SubfolderName, UrlName), AsHtmlString());
			SaveAdditional();
		}

		public override void SaveAdditional()
		{
			File.WriteAllText(string.Format("{0}/static/code_{1}.html", FolderName, UrlName),
				File.ReadAllText("code-template.html").Replace("{0}", LangId).Replace("{1}", Source));
		}

		public override EdxReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return File.ReadAllText("iframe-template.html")
				.Replace("{0}", "code_" + UrlName)
				.Replace("{1}", "(function (obj) { obj.style.height = obj.contentWindow.document.documentElement.scrollHeight + 'px'; })(this);");
		}
	}
}
