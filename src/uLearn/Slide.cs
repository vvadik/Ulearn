using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class Slide
	{
		public readonly SlideBlock[] Blocks;

		public Slide(IEnumerable<SlideBlock> blocks)
		{
			Blocks = blocks.ToArray();
		}
	}
}