using System.Collections.Generic;
using System.Xml.Serialization;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class ExerciseBlock : SlideBlock
	{
		private List<string> hintsMd;
		// “о, что показываетс€ пользователю в начале выполенени€ задани€
		public string ExerciseInitialCode { get; set; }
		// “о, что будет выполн€тьс€ дл€ проверки задани€
		public string ExerciseCode { get; set; }
		// »ндекс внутри ExerciseCode, куда нужно вставить код пользовател€.
		public int IndexToInsertSolution { get; set; }
		// ќжидаемый корректный вывод программы
		public string ExpectedOutput { get; set; }
		// ≈сли это вставить в ExerciseCode по индексу IndexToInsertSolution и выполнить полученный код, он должен вывести ExpectedOutput
		public string EthalonSolution { get; set; }
		public string Lang { get; set; }
		public string LangVer { get; set; }
		public string CommentAfterExerciseIsSolved { get; set; }
		public bool HideExpectedOutputOnError { get; set; }
		public string ValidatorName { get; set; }

		public List<string> HintsMd
		{
			get { return hintsMd = hintsMd ?? new List<string>(); }
			set { hintsMd = value; }
		}
		
		[XmlIgnore]
		public SolutionBuilder Solution { get { return new SolutionBuilder(IndexToInsertSolution, ExerciseCode, ValidatorName); } }

		protected bool Equals(ExerciseBlock other)
		{
			return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsMd, other.HintsMd);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ExerciseBlock)obj);
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