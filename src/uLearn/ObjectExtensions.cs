using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace uLearn
{
	public static class ObjectExtensions
	{
		private static readonly XmlWriterSettings defaultSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
		private static readonly XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

		public static string XmlSerialize(this object o, bool removeWhitespaces = false)
		{
			var settings = defaultSettings;
			if (removeWhitespaces)
			{
				settings.Indent = false;
				settings.NewLineHandling = NewLineHandling.None;
			}

			using (var ms = new MemoryStream())
			using (var writer = XmlWriter.Create(ms, settings))
			{
				var s = new XmlSerializer(o.GetType());
				s.Serialize(writer, o, ns);
				ms.Flush();
				ms.Seek(0, SeekOrigin.Begin);
				var sr = new StreamReader(ms);
				return sr.ReadToEnd();
			}
		}
	}
}
