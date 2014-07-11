using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class Slide
	{
		public readonly string Title;
		public readonly SlideBlock[] Blocks;
		public readonly SlideInfo Info;


		public Slide(IEnumerable<SlideBlock> blocks, SlideInfo info, string title)
		{
			Blocks = blocks.ToArray();
			Info = info;
			Title = title;
		}
	}
}