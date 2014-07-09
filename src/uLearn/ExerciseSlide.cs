using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public SlideBlock Exercise { get; private set; }
		public string ExpectedOutput { get; private set; }
		public SolutionForTesting Solution { get; private set; }
		public string[] HintsHtml { get; private set; }

		public ExerciseSlide(IEnumerable<SlideBlock> blocks, SlideBlock exercise, string expectedOutput,
			IEnumerable<string> hints, SolutionForTesting solution)
			: base(blocks)
		{
			Exercise = exercise;
			ExpectedOutput = expectedOutput;
			Solution = solution;
			HintsHtml = hints.Select(Md.ToHtml).ToArray();
		}

		protected bool Equals(ExerciseSlide other)
		{
			return Equals(Exercise, other.Exercise) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsHtml, other.HintsHtml);
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
				hashCode = (hashCode*397) ^ (ExpectedOutput != null ? ExpectedOutput.GetHashCode() : 0);
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