using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ulearn.Common.Extensions;

namespace Ulearn.Common
{
	public class LanguageLaunchInfo
	{
		public string Compiler { get; set; }
		public string CompileCommand { get; set; }
		public string RunCommand { get; set; }
	}
	
	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum Language : short
	{
		[XmlEnum("csharp")]
		[Lexer("csharp")]
		CSharp = 1,

		[XmlEnum("python2")]
		[Lexer("py2")]
		Python2 = 2,

		[XmlEnum("python3")]
		[Lexer("py3")]
		Python3 = 3,

		[XmlEnum("java")]
		[Lexer("java")]
		Java = 4,

		[XmlEnum("javascript")]
		[Lexer("js")]
		JavaScript = 5,

		[XmlEnum("html")]
		[Lexer("html")]
		Html = 6,

		[XmlEnum("typescript")]
		[Lexer("ts")]
		TypeScript = 7,

		[XmlEnum("css")]
		[Lexer("css")]
		Css = 8,

		[XmlEnum("haskell")]
		[Lexer("haskell")]
		Haskell = 9,
		
		[XmlEnum("cpp")]
		[Lexer("cpp")]
		Cpp = 10,

		[XmlEnum("c")]
		[Lexer("c")]
		C = 11,

		[XmlEnum("text")]
		[Lexer("text")]
		Text = 100,
	}

	public class LexerAttribute : Attribute
	{
		public readonly string lexer;
		public LexerAttribute(string lexer)  
		{  
			this.lexer = lexer;
		}  
	}

	public static class LanguageHelpers
	{
		private static readonly Dictionary<string, Language> extensions = new Dictionary<string, Language>
		{
			{ ".cs", Language.CSharp },
			{ ".py", Language.Python3 },
			{ ".py2", Language.Python2 },
			{ ".py3", Language.Python3 },
			{ ".html", Language.Html },
			{ ".css", Language.Css },
			{ ".js", Language.JavaScript },
			{ ".ts", Language.TypeScript },
			{ ".java", Language.Java },
			{ ".hs", Language.Haskell },
			{ ".c", Language.C },
			{ ".cpp", Language.Cpp },
			{ ".txt", Language.Text }
		};

		public static Language GuessByExtension(string extension)
		{
			if (extensions.ContainsKey(extension))
				return extensions[extension];

			throw new ArgumentException(
				$"Can't guess programming language by file extension. Unknown file extension: {extension}\n" +
				$"Known extensions are {string.Join(", ", extensions.Keys)}."
			);
		}

		public static Language GuessByExtension(FileInfo file)
		{
			return GuessByExtension(file.Extension);
		}

		public static Language ParseFromXml(string language)
		{
			var serializer = new XmlSerializer(typeof(Language));

			/* XmlSerializer expects XML so wrap what you got in xml tags */
			var xmlBytes = Encoding.ASCII.GetBytes($"<Language>{language.EscapeHtml()}</Language>");
			using (var ms = new MemoryStream(xmlBytes))
				return (Language)serializer.Deserialize(ms);
		}

		public static Language ParseByName(string language)
		{
			return ParseFromXml(language);
		}

		public static bool TryParseByName(string language, out Language value)
		{
			try
			{
				value = ParseByName(language);
				return true;
			}
			catch (Exception e)
			{
				value = Language.Text;
				return false;
			}
		}
	}

	public static class LanguageExtensions
	{
		public static bool HasAutomaticChecking(this Language language)
		{
			return language == Language.CSharp || language == Language.JavaScript || language == Language.Python3 || language == Language.Haskell;
		}

		public static string GetName(this Language language)
		{
			return language.GetXmlEnumName();
		}

		public static string GetName(this Language? language)
		{
			if (language.HasValue)
				return language.Value.GetName();
			return "";
		}
	}
}