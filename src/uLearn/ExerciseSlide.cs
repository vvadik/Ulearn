using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Xml;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public string ExerciseInitialCode { get; private set; }
		public string ExpectedOutput { get; private set; }
		public SolutionForTesting Solution { get; private set; }
		public string[] HintsHtml { get; private set; }

		public ExerciseSlide(IEnumerable<SlideBlock> blocks, string exerciseInitialCode, string expectedOutput, IEnumerable<string> hints, SolutionForTesting solution, SlideInfo slideInfo, string title, string id)
			: base(blocks, slideInfo, title, id)
		{
			ExerciseInitialCode = exerciseInitialCode ?? "";
			ExpectedOutput = expectedOutput;
			Solution = solution;
			HintsHtml = hints.Select(Md.ToHtml).ToArray();
		}

		protected bool Equals(ExerciseSlide other)
		{
			return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsHtml, other.HintsHtml);
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
				int hashCode = (ExerciseInitialCode != null ? ExerciseInitialCode.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (ExpectedOutput != null ? ExpectedOutput.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (HintsHtml != null ? HintsHtml.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", ExerciseInitialCode, HintsHtml);
		}
	}
}