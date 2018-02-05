using System;

namespace uLearn.Model.Blocks
{
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