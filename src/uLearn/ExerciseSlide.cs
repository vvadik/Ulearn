using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public override bool ShouldBeSolved { get { return true; } }

		public ExerciseBlock Exercise { get; set; }
		
		public ExerciseSlide(
			List<SlideBlock> blocks,
			SlideInfo slideInfo,
			string title, string id)
			: base(blocks, slideInfo, title, id)
		{
			MaxScore = 5;
			Exercise = blocks.OfType<ExerciseBlock>().SingleOrDefault();
		}

		public override string ToString()
		{
			return string.Format("ExerciseSlide: {0}", Exercise);
		}
	}
}