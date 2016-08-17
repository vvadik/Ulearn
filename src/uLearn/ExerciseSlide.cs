using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public override bool ShouldBeSolved { get { return true; } }

		public ExerciseBlock Exercise { get; }
		
		public ExerciseSlide(
			List<SlideBlock> blocks,
			SlideInfo slideInfo,
			string title, Guid id)
			: base(blocks, slideInfo, title, id)
		{
			Exercise = blocks.OfType<ExerciseBlock>().Single();
			MaxScore = Exercise.MaxScore;
		}

		public override string ToString()
		{
			return string.Format("ExerciseSlide: {0}", Exercise);
		}
	}
}