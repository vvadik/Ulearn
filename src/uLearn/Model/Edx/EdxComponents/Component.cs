using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
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
			File.WriteAllText(string.Format("{0}/{1}/{2}.xml", FolderName, SubfolderName, UrlName), this.XmlSerialize());
		}

		public virtual void SaveAdditional()
		{
		}

		public virtual XmlElement AsXmlElement()
		{
			var doc = new XmlDocument();
			doc.LoadXml("<p>" + AsHtmlString() + "</p>");
			return doc.DocumentElement;
		}

		public abstract EdxReference GetReference();
		public abstract string AsHtmlString();
	}
}
