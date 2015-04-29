using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.CSharp;

namespace uLearn
{
    public abstract class SlideBlock
    {
	    public virtual IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress)
	    {
		    yield return this;
	    }
    }

	[XmlType("gallery-images")]
    public class ImageGaleryBlock : SlideBlock
    {
		[XmlElement("image")]
		public string[] ImageUrls { get; set; }

		public ImageGaleryBlock(string[] images)
	    {
		    ImageUrls = images;
	    }

		public ImageGaleryBlock()
		{
		}

		public override string ToString()
        {
            return string.Format("Images {0}", string.Join("\n", ImageUrls));
        }
    }

	[XmlType("tex")]
    public class TexBlock : SlideBlock
    {
		[XmlElement("line")]
		public string[] TexLines { get; set; }

		public TexBlock(string[] texLines)
        {
            TexLines = texLines;
        }

		public TexBlock()
	    {
	    }

		public override string ToString()
        {
            return string.Format("Tex {0}", string.Join("\n", TexLines));
        }
    }

	[XmlType("md")]
	public class MdBlock : SlideBlock
    {
		[XmlText]
		public string Markdown { get; set; }

		public MdBlock(string markdown)
        {
            if (markdown != null)
                Markdown = markdown.TrimEnd();
        }

		public MdBlock()
	    {
	    }

		public override string ToString()
        {
            return string.Format("Markdown {0}", Markdown);
        }
    }

	[XmlType("code")]
    public class CodeBlock : SlideBlock
    {
		[XmlText]
		public string Code { get; set; }
		[XmlAttribute("lang")]
		public string Lang { get; set; }

        public CodeBlock(string code, string lang)
        {
            Code = code;
            Lang = lang;
        }

	    public CodeBlock()
	    {
	    }

	    public override string ToString()
        {
            return string.Format("{0} code {1}", Lang, Code);
        }
    }

	[XmlType("youtube")]
	public class YoutubeBlock : SlideBlock
    {
		[XmlText]
		public string VideoId { get; set; }

		public YoutubeBlock(string videoId)
		{
			VideoId = videoId;
		}

		public YoutubeBlock()
	    {
	    }

		public override string ToString()
        {
            return string.Format("Video {0}", VideoId);
        }
    }

	[XmlType("include-code")]
	public class IncludeCodeBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		[XmlElement("label")]
		public string[] Labels { get; set; }

		public IncludeCodeBlock(string file)
		{
			File = file;
		}

		public IncludeCodeBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress)
		{
			var content = fs.GetContent(File);
			var lang = Path.GetExtension(File).Trim('.');
			if (lang == "cs")
				lang = "csharp";
			if (Labels == null || Labels.Length == 0)
			{
				yield return new CodeBlock(content, lang);
			}
			else
			{
				var tree = CSharpSyntaxTree.ParseText(content);
				var objects = tree.GetRoot().DescendantNodes()
					.OfType<MemberDeclarationSyntax>()
					.Where(node => node is ClassDeclarationSyntax || node is MethodDeclarationSyntax)
					.Where(node => !node.HasAttribute<HideOnSlideAttribute>())
					.GroupBy(node => node.Identifier().ValueText)
					.ToDictionary(
						nodes => nodes.Key, 
						nodes => String.Join("\r\n\r\n", nodes.Select(node => node.ToPrettyString()))
					);
				yield return new CodeBlock(String.Join("\r\n\r\n", Labels.Select(s => objects[s])), lang);
			}
		}
	}

	[XmlType("include-md")]
	public class IncludeMdBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		public IncludeMdBlock(string file)
		{
			File = file;
		}

		public IncludeMdBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress)
		{
			yield return new MdBlock(fs.GetContent(File));
		}
	}

	[XmlType("gallery")]
	public class IncludeImageGalleryBlock : SlideBlock
	{
		[XmlText]
		public string Directory { get; set; }

		public IncludeImageGalleryBlock(string directory)
		{
			Directory = directory;
		}

		public IncludeImageGalleryBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress)
		{
			yield return new ImageGaleryBlock(fs.GetFilenames(Directory));
		}
	}

	[XmlType("include-blocks")]
	public class IncludeBlocksBlock : SlideBlock
	{
		[XmlAttribute("file")]
		public string File { get; set; }

		public IncludeBlocksBlock(string file)
		{
			File = file;
		}

		public IncludeBlocksBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress)
		{
			if (filesInProgress.Contains(File))
				throw new Exception("Cyclic dependency");

			var xmlStream = new StringReader(fs.GetContent(File));
			var serializer = new XmlSerializer(typeof(SlideBlock[]));
			var slideBlocks = (SlideBlock[])serializer.Deserialize(xmlStream);
			var newInProgress = filesInProgress.Add(File);
			return slideBlocks.SelectMany(b => b.BuildUp(fs, newInProgress));
		}
	}

	public static class SlideBlockExtensions
    {
        public static bool IsCode(this SlideBlock block)
        {
            return block is CodeBlock;
        }

        public static string Text(this SlideBlock block)
        {
            var md = block as MdBlock;
            if (md != null)
                return md.Markdown;
            var code = block as CodeBlock;
            if (code != null)
                return code.Code;
            throw new Exception(block.ToString());
        }
    }
}