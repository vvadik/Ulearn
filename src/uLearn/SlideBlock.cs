using System;

namespace uLearn
{
	public abstract class SlideBlock
	{
	}

	public class MdBlock : SlideBlock
	{
		public readonly string Markdown;

		public MdBlock(string markdown)
		{
			Markdown = markdown.TrimEnd();
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