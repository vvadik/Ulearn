using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("html")]
	public class HtmlComponent : Component
	{
		[XmlIgnore]
		public string Source;

		[XmlIgnore]
		public string LocalFolder;

		[XmlIgnore]
		public List<string> LocalFiles;

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

		public HtmlComponent(string folderName, string urlName, string displayName, string filename, string source)
		{
			FolderName = folderName;
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			Source = source;
		}

		public HtmlComponent(string folderName, string urlName, string displayName, string filename, string source, string localFolder, List<string> localFiles)
		{
			FolderName = folderName;
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			Source = source;
			LocalFolder = localFolder;
			LocalFiles = localFiles;
		}

		public override void Save()
		{
			base.Save();
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", FolderName, SubfolderName, UrlName), Source);
			SaveAdditional();
		}

		public override void SaveAdditional()
		{
			try
			{
				foreach (var localFile in LocalFiles)
					File.Copy(string.Format("{0}/{1}", LocalFolder, localFile), string.Format("{0}/static/{1}_{2}", FolderName, UrlName, localFile.Replace("/", "_")));
			}
			catch (Exception)
			{
			}
		}

		public override EdxReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return "<p>" + Source + "</p>";
		}
	}
}
