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

		[XmlIgnore]
		public override string SubfolderName => "html";

		public CodeComponent()
		{
		}

		public CodeComponent(string urlName, string displayName, string filename, string langId, string source)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			LangId = langId;
			Source = source;
		}

		public override void Save(string folderName)
		{
			base.Save(folderName);
			File.WriteAllText($"{folderName}/{SubfolderName}/{UrlName}.html", AsHtmlString());
		}

		public override void SaveAdditional(string folderName)
		{
			File.WriteAllText(
				$"{folderName}/assets/code_{UrlName}.html",
				File.ReadAllText($"{Utils.GetRootDirectory()}/templates/code.html").Replace("{0}", LangId).Replace("{1}", Source)
			);
		}

		public override EdxReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return File.ReadAllText($"{Utils.GetRootDirectory()}/templates/iframe.html")
				.Replace("{0}", "code_" + UrlName)
				.Replace("{1}", "(function (obj) { obj.style.height = obj.contentWindow.document.documentElement.scrollHeight + 'px'; })(this);");
		}
	}
}
