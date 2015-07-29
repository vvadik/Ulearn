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
		}

		[XmlIgnore]
		public Component[] Subcomponents;

		public HtmlComponent()
		{
		}

		public HtmlComponent(string urlName, string displayName, string filename, string source)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			Source = source;
		}

		public HtmlComponent(string urlName, string displayName, string filename, string source, string localFolder, List<string> localFiles)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			Source = source;
			LocalFolder = localFolder;
			LocalFiles = localFiles;
		}

		public override void Save(string folderName)
		{
			base.Save(folderName);
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", folderName, SubfolderName, UrlName), Source);
		}

		public override void SaveAdditional(string folderName)
		{
			if (Subcomponents != null)
				foreach (var subcomponent in Subcomponents)
					subcomponent.SaveAdditional(folderName);
			try
			{
				foreach (var localFile in LocalFiles)
					File.Copy(string.Format("{0}/{1}", LocalFolder, localFile), string.Format("{0}/static/{1}_{2}", folderName, UrlName, localFile.Replace("/", "_")));
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

		public static HtmlComponent Load(string folderName, string urlName)
		{
			var component = new FileInfo(string.Format("{0}/html/{1}.xml", folderName, urlName)).DeserializeXml<HtmlComponent>();
			component.UrlName = urlName;
			component.Source = File.ReadAllText(string.Format("{0}/html/{1}.html", folderName, component.Filename));
			return component;
		}
	}
}
