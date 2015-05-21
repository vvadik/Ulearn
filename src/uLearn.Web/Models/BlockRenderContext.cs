using System;

namespace uLearn.Web.Models
{
	public class BlockRenderContext
	{
		public Course Course { get; private set; }
		public Slide Slide { get; private set; }
		public string BaseUrl { get; private set; }
		public dynamic[] BlockData { get; private set; }

		public dynamic GetBlockData(SlideBlock block)
		{
			var index = Array.IndexOf(Slide.Blocks, block);
			if (index < 0)
				throw new ArgumentException("No block " + block + " in slide " + Slide);
			return BlockData[index];

		}

		public BlockRenderContext(Course course, Slide slide, string baseUrl, dynamic[] blockData)
		{
			Course = course;
			Slide = slide;
			BaseUrl = baseUrl;
			BlockData = blockData;
			if (BlockData.Length != Slide.Blocks.Length)
				throw new ArgumentException("BlockData.Length should be slide.Blocks.Length");
		}
	}
}