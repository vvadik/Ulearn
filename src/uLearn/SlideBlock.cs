using System;
using System.Net;

namespace uLearn
{
	public abstract class SlideBlock
	{
	}

	public class TexBlock : SlideBlock
	{
		public string[] TexLines { get; set; }

		public TexBlock(string[] texLines)
		{
			TexLines = texLines;
		}

		public override string ToString()
		{
			return string.Format("Tex {0}", string.Join("\n", TexLines));
		}
	}

	public class MdBlock : SlideBlock
	{
		public string Markdown
		{
			get { return markdown ?? (markdown = LoadMarkdown()); }
		}

		private string LoadMarkdown()
		{
			try
			{
			    using (var webClient = new WebClient())
			    {
                    return webClient.DownloadData(url).AsUtf8();   
			    }
			}
			catch (WebException e)
			{
				return "Error: Content is not available.\n\n" + e;
			}
		}

		private string markdown;
		private readonly string url;

		private MdBlock(string markdown, string url)
		{
			if (markdown != null)
				this.markdown = markdown.TrimEnd();
			this.url = url;

		}

		public static MdBlock FromText(string markdown)
		{
			return new MdBlock(markdown, null);
		}

		public static MdBlock FromUrl(string url)
		{
			return new MdBlock(null, url);
		}

		public override string ToString()
		{
			return string.Format("Markdown {0}", Markdown);
		}
	}


	public class CodeBlock : SlideBlock
	{
		public readonly string Code;

		public CodeBlock(string code)
		{
			Code = code;
		}

		public override string ToString()
		{
			return string.Format("Code {0}", Code);
		}
	}

	public class YoutubeBlock : SlideBlock
	{
		public readonly string VideoId;

		public YoutubeBlock(string videoId)
		{
			VideoId = videoId;
		}

		public override string ToString()
		{
			return string.Format("Video {0}", VideoId);
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
			if (md != null) return md.Markdown;
			var code = block as CodeBlock;
			if (code != null) return code.Code;
			throw new Exception(block.ToString());
		}
	}
}