using System.Collections.Generic;

namespace uLearn
{
	public class ExerciseSlide : Slide
	{
		// То, что показывается пользователю в начале выполенения задания
		public string ExerciseInitialCode { get; set; }
		// То, что будет выполняться для проверки задания
		public string ExerciseCode { get; set; }
		// Индекс внутри ExerciseCode, куда нужно вставить код пользователя.
		public int IndexToInsertSolution { get; set; }
		// Ожидаемый корректный вывод программы
		public string ExpectedOutput { get; set; }
		// Если это вставить в ExerciseCode по индексу IndexToInsertSolution и выполнить полученный код, он должен вывести ExpectedOutput
		public string EthalonSolution { get; set; }
		public string Lang { get; set; }
		public string CommentAfterExerciseIsSolved { get; set; }
		public bool HideExpectedOutputOnError { get; set; }
		public string ValidatorName { get; set; }
		public List<string> HintsMd { get; set; }

		public SolutionBuilder Solution { get { return new SolutionBuilder(IndexToInsertSolution, ExerciseCode, ValidatorName); } }
		public override bool ShouldBeSolved { get { return true; } }

		public ExerciseSlide(
			string lang,
			IEnumerable<SlideBlock> blocks,
			SlideInfo slideInfo,
			string title, string id)
			: base(blocks, slideInfo, title, id)
		{
			Lang = lang;
			ExerciseInitialCode = "";
			HintsMd = new List<string>();
			MaxScore = 5;
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
			return Equals((ExerciseSlide)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (ExerciseInitialCode != null ? ExerciseInitialCode.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ExpectedOutput != null ? ExpectedOutput.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (HintsMd != null ? HintsMd.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", ExerciseInitialCode, HintsMd);
		}
	}
}