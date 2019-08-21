using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Ulearn.Core
{
	public class XmlValidator
	{
		private readonly XmlReaderSettings settings;

		public XmlValidator()
		{
			var schemaSet = new XmlSchemaSet();

			using (var r = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + "../" + "schema.xsd"))
			{
				schemaSet.Add(XmlSchema.Read(r, null));
			}

			schemaSet.CompilationSettings = new XmlSchemaCompilationSettings();
			schemaSet.Compile();

			settings = new XmlReaderSettings
			{
				CloseInput = true,
				ValidationType = ValidationType.Schema,
				Schemas = schemaSet,
				ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings |
								XmlSchemaValidationFlags.ProcessIdentityConstraints |
								XmlSchemaValidationFlags.ProcessInlineSchema |
								XmlSchemaValidationFlags.ProcessSchemaLocation
			};
		}

		public string ValidateSlideFile(FileInfo file)
		{
			var log = new List<string>();
			log.Add(file.Directory != null ? $"Errors in slide {file.Directory.Name}/{file.Name}" : $"Errors in slide {file.FullName}");

			void Action(object sender, ValidationEventArgs e)
			{
				var text = $"	[Line: {e.Exception?.LineNumber}, Column: {e.Exception?.LinePosition}]: {e.Message}";
				log.Add(text);
			}

			settings.ValidationEventHandler += Action;
			using (var validatingReader = XmlReader.Create(file.FullName, settings))
			{
				var x = new XmlDocument();
				x.Load(validatingReader);

				while (validatingReader.Read()) 
				{
				}
			}

			return log.Count > 1 ? string.Join("\n", log) : null;
		}

		public string ValidateSlidesFiles(List<FileInfo> files)
		{
			return string.Join("\n", files.Select(ValidateSlideFile).Where(x => x != null));
		}
	}
}