using System.Linq;
using System.Xml.Linq;

namespace Ulearn.Common
{
	public static class XmlUtils
	{
		public static string RemoveAllNamespaces(string xmlDocument)
		{
			var xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

			return xmlDocumentWithoutNs.ToString();
		}
		
		private static XElement RemoveAllNamespaces(XElement xmlElement)
		{
			var child = xmlElement.Nodes().Select(node => node is XElement element ? RemoveAllNamespaces(element) : node);
			var attributes = xmlElement.HasAttributes
				? xmlElement.Attributes().Where(a => !a.IsNamespaceDeclaration).Select(a => new XAttribute(a.Name.LocalName, a.Value))
				: null;
			
			return new XElement(xmlElement.Name.LocalName, child, attributes);
		}
	}
}