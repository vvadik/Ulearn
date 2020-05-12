using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	// [XmlType("markdown")]
	// [XmlRoot("markdown", Namespace = "https://ulearn.me/schema/v2")]
	public class MarkdownBlock : SlideBlock, IXmlSerializable, IApiConvertibleSlideBlock
	{
		private string markdown;

		[XmlText]
		public string Markdown
		{
			get => markdown;
			set => markdown = value.RemoveCommonNesting();
		}
		
		public SlideBlock[] InnerBlocks { get; set; } // может содержать MarkdownBlock или CodeBlock

		public MarkdownBlock(string markdown)
		{
			if (markdown != null)
				Markdown = markdown.TrimEnd();
		}

		public MarkdownBlock()
		{
		}

		public string RenderMarkdown(string courseId, Guid slideId, string baseUrl)
		{
			return GetMarkdownWithReplacedLinksToStudentZips(courseId, slideId).RenderMarkdown(baseUrl);
		}

		public string RenderMarkdown(string courseId, Guid slideId, FileInfo sourceFile, string baseUrl = "")
		{
			return GetMarkdownWithReplacedLinksToStudentZips(courseId, slideId).RenderMarkdown(sourceFile, baseUrl);
		}

		/* Replace links to (/Exercise/StudentZip) and to (ExerciseZip): automagically add courseId and slideId */
		private string GetMarkdownWithReplacedLinksToStudentZips(string courseId, Guid slideId)
		{
			if (string.IsNullOrEmpty(Markdown))
				return "";
			var studentZipFullPath = $"(/Exercise/StudentZip?courseId={courseId}&slideId={slideId})";
			return Markdown.Replace("(/Exercise/StudentZip)", studentZipFullPath).Replace("(ExerciseZip)", studentZipFullPath);
		}

		public override string ToString()
		{
			return $"Markdown {Markdown}";
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			var slideDirectory = slide.Info.SlideFile.Directory;
			var directoryRelativePath = "/Courses/" + courseId + "/" + slideDirectory.GetRelativePath(coursePackageRoot.FullName);
			var baseUrl = ulearnBaseUrl + directoryRelativePath.Replace('\\', '/');
			var html = RenderMarkdown(courseId, slide.Id, baseUrl);
			var urlName = slide.NormalizedGuid + componentIndex;
			return new HtmlComponent(urlName, displayName, urlName, html);
		}

		public Component ToEdxComponent(string urlName, string displayName, string directoryName)
		{
			var htmlWithUrls = Markdown.GetHtmlWithUrls("/static/" + urlName + "_");
			return new HtmlComponent(urlName, displayName, urlName, htmlWithUrls.Item1, directoryName, htmlWithUrls.Item2);
		}

		private static readonly Regex codeTextareaRegex = new Regex("<textarea[^>]+data-lang='(?<lang>[^']*)'[^>]*>(?<code>[^<]*)</textarea>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		IEnumerable<IApiSlideBlock> IApiConvertibleSlideBlock.ToApiSlideBlocks(ApiSlideBlockBuildingContext context)
		{
			// К этому моменту BuildUp уже вызван, для InnerBlocks созданы отдельные блоки, InnerBlocks обрабатывать не нужно
			var renderedMarkdown = RenderMarkdown(context.CourseId, context.SlideId, context.BaseUrl);
			var matches = codeTextareaRegex.Matches(renderedMarkdown);
			var previousMatchEnd = 0;
			foreach (var match in matches.Cast<Match>())
			{
				var markdownPart = renderedMarkdown.Substring(previousMatchEnd, match.Index - previousMatchEnd);
				if (!string.IsNullOrWhiteSpace(markdownPart))
					yield return new HtmlBlock(markdownPart)
				{
					Hide = Hide,
					FromMarkdown = true
				};
				var langStr = match.Groups["lang"].Value;
				var lang = (Language)Enum.Parse(typeof(Language), langStr, true);
				var codeXmlEncoded = match.Groups["code"].Value;
				var code = WebUtility.HtmlDecode(codeXmlEncoded);
				yield return new CodeBlock(code, lang) { Hide = Hide };
				previousMatchEnd = match.Index + match.Length;
			}
			var lastPart = previousMatchEnd == 0
				? renderedMarkdown
				: renderedMarkdown.Substring(previousMatchEnd, renderedMarkdown.Length - previousMatchEnd);
			if (!string.IsNullOrWhiteSpace(lastPart))
				yield return new HtmlBlock(lastPart)
				{
					Hide = Hide,
					FromMarkdown = true
				};
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			return InnerBlocks?.SelectMany(b => b.BuildUp(context, filesInProgress)) ?? new[] { this };
		}

		public override string TryGetText()
		{
			return Markdown;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			Hide = reader.GetAttribute("hide").IsOneOf("true", "1");
			var blocks = ReadBlocks(Hide, reader).ToArray();
			if (blocks.Length == 1 && blocks[0].GetType() == typeof(MarkdownBlock))
			{
				var mb = (MarkdownBlock)blocks[0];
				Markdown = mb.Markdown;
				Hide = mb.Hide;
			}
			else
				InnerBlocks = blocks;
		}

		private IEnumerable<SlideBlock> ReadBlocks(bool hide, XmlReader reader)
		{
			var tagName = reader.Name;
			if (reader.IsEmptyElement)
			{
				reader.Read();
				yield break;
			}

			reader.Read();
			while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == tagName))
			{
				if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA)
				{
					yield return new MarkdownBlock(reader.ReadContentAsString()) { Hide = hide };
				}
				else if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.LocalName == "note")
					{
						yield return new MarkdownBlock
						{
							Hide = true,
							Markdown = reader.ReadElementContentAsString()
						};
					}
					else if (reader.LocalName == "code")
					{
						var languageAttribute = reader.GetAttribute("language");
						Language? language = null;
						if (!string.IsNullOrEmpty(languageAttribute))
							language = LanguageHelpers.ParseFromXml(languageAttribute);
						yield return new CodeBlock(reader.ReadElementContentAsString(), language) { Hide = hide };
					}
					else
						throw new NotSupportedException(
							$"Invalid tag inside of <markdown>: {reader.LocalName}. Supported tags inside <markdown> are <note> and <code>."
						);
				}
				else
					reader.Read();
			}

			reader.Read();
		}

		public void WriteXml(XmlWriter writer)
		{
			if (Hide)
				writer.WriteAttributeString("hide", "true");
			writer.WriteString(markdown);
		}
	}
}