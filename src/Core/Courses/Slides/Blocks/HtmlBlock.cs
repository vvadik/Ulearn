using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	//[XmlType("html")]
	public class HtmlBlock : SlideBlock, IXmlSerializable
	{
		public string Content { get; private set; } = "";

		public HtmlBlock()
		{
		}
		
		public HtmlBlock(string content)
		{
			Content = content;
		}

		public override string ToString()
		{
			return $"Html";
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotSupportedException();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			
			if (reader.IsEmptyElement)
			{
				reader.Read();
				return;
			}

			var innerXml = reader.ReadInnerXml();
			Content = RemoveXmlNamespaces(innerXml.RemoveCommonNesting());
		}

		private string RemoveXmlNamespaces(string innerXmlContent)
		{
			/* Add outer tag, otherwise XML is not correct and XmlUtils can't parse it and remove namespace */
			var xml = $"<node>{innerXmlContent}</node>";
			
			var xmlWithoutNs = XmlUtils.RemoveAllNamespaces(xml);
			
			/* Delete outer tag */
			if (xmlWithoutNs.StartsWith("<node>"))
				xmlWithoutNs = xmlWithoutNs.Remove(0, "<node>".Length);
			if (xmlWithoutNs.EndsWith("</node>"))
				xmlWithoutNs = xmlWithoutNs.Remove(xmlWithoutNs.Length - "</node>".Length);

			return xmlWithoutNs;
		}

		public void WriteXml(XmlWriter writer)
		{
			if (Hide)
				writer.WriteAttributeString("hide", "true");
			writer.WriteRaw(Content);
		}
	}
}