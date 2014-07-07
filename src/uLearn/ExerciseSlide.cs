using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public SlideBlock Exercise { get; private set; }
		public SlideBlock TestBlock { get; private set; }
		public string[] HintsHtml { get; private set; }

		public ExerciseSlide(IEnumerable<SlideBlock> blocks, SlideBlock exercise, SlideBlock testBlock,
			IEnumerable<string> hints)
			: base(blocks)
		{
			Exercise = exercise;
			TestBlock = testBlock;
			HintsHtml = hints.Select(Md.ToHtml).ToArray();
		}

		protected bool Equals(ExerciseSlide other)
		{
			return Equals(Exercise, other.Exercise) && Equals(TestBlock, other.TestBlock) && Equals(HintsHtml, other.HintsHtml);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ExerciseSlide) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (Exercise != null ? Exercise.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (TestBlock != null ? TestBlock.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (HintsHtml != null ? HintsHtml.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", Exercise, HintsHtml);
		}
	}
}