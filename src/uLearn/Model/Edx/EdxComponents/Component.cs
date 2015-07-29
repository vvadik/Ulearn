using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	public abstract class Component : EdxItem
	{
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
