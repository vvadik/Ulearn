using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;

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
		public int MaxScore { get; protected set; }


		public Slide(IEnumerable<SlideBlock> blocks, SlideInfo info, string title, string id)
		{
			try
			{
				Info = info;
				Title = title;
				Id = id;
				MaxScore = 0;
				Blocks = blocks.ToArray();
				foreach (var block in Blocks)
					block.Validate();
			}
			catch (Exception e)
			{
				throw new FormatException(string.Format("Error in slide {0} (id: {1}). {2}", title, id, e.Message), e);
			}
		}

		public override string ToString()
		{
			return string.Format("Title: {0}, Id: {1}, MaxScore: {2}", Title, Id, MaxScore);
		}
	}
}