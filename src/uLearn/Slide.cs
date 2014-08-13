using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class Slide
	{
		public readonly string Title;
		public readonly SlideBlock[] Blocks;
		public readonly SlideInfo Info;
		public int Index { get { return Info.Index; }}
		public readonly string Id;
		public virtual bool ShouldBeSolved { get { return false; } }


		public Slide(IEnumerable<SlideBlock> blocks, SlideInfo info, string title, string id)
		{
			Blocks = blocks.ToArray();
			Info = info;
			Title = title;
			Id = id;
		}
	}
}