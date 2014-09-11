using System.Collections.Generic;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		public string ExerciseInitialCode { get; set; }
		public string EthalonSolution { get; set; }
		public string ExpectedOutput { get; set; }
		public string CommentAfterExerciseIsSolved { get; set; }
		public bool HideExpectedOutputOnError { get; set; }
		public SolutionBuilder Solution { get; set; }
		public List<string> HintsMd { get; set; }
		public override bool ShouldBeSolved { get { return true; } }

		public ExerciseSlide(
			IEnumerable<SlideBlock> blocks,
			SlideInfo slideInfo, 
			string title, string id)
			: base(blocks, slideInfo, title, id)
		{
			ExerciseInitialCode = "";
			HintsMd = new List<string>();
		}

		protected bool Equals(ExerciseSlide other)
		{
			return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsMd, other.HintsMd);
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
				hashCode = (hashCode*397) ^ (HintsMd != null ? HintsMd.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", ExerciseInitialCode, HintsMd);
		}
	}
}