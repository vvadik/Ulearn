using System.Collections.Generic;
using System.Linq;

namespace SharpLessons
{
	public class ExerciseSlide : Slide
	{
		public SlideBlock Exercise { get; private set; }
		public string[] HintsHtml { get; private set; }

		public ExerciseSlide(string id, IEnumerable<SlideBlock> blocks, SlideBlock exercise, IEnumerable<string> hints)
			: base(id, blocks)
		{
			Exercise = exercise;
			HintsHtml = hints.Select(Md.ToHtml).ToArray();
		}

		protected bool Equals(ExerciseSlide other)
		{
			return Equals(Exercise, other.Exercise) && Equals(HintsHtml, other.HintsHtml);
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
				return ((Exercise != null ? Exercise.GetHashCode() : 0)*397) ^ (HintsHtml != null ? HintsHtml.GetHashCode() : 0);
			}
		}

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", Exercise, HintsHtml);
		}
	}
}