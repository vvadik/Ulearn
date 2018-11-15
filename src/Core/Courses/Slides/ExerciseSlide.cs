using System;
using System.Collections.Generic;
using System.Linq;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Slides
{
	public class ExerciseSlide : Slide
	{
		public override bool ShouldBeSolved => true;

		public ExerciseBlock Exercise { get; }

		public ExerciseSlide(
			List<SlideBlock> blocks,
			SlideInfo slideInfo,
			string title,
			Guid id,
			SlideMetaDescription meta)
			: base(blocks, slideInfo, title, id, meta)
		{
			Exercise = blocks.OfType<ExerciseBlock>().Single();
			MaxScore = Exercise.MaxScore;
			ScoringGroup = Exercise.ScoringGroup ?? "";
		}

		public override string ToString()
		{
			return $"ExerciseSlide: {Exercise}";
		}
	}
}