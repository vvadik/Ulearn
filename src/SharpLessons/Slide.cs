using System.Collections.Generic;
using System.Linq;

namespace SharpLessons
{
	public class Slide
	{
		public readonly SlideBlock[] Blocks;
		public readonly string Id;

		public Slide(string id, IEnumerable<SlideBlock> blocks)
		{
			Id = id;
			Blocks = blocks.ToArray();
		}
	}
}