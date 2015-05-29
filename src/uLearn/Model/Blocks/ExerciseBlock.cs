using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using uLearn.CSharp;

namespace uLearn.Model.Blocks
{
	[XmlType("execirse")]
	public class ExerciseBlock : IncludeCode
	{
		[XmlElement("inital-code")]
		public string ExerciseInitialCode { get; set; }

		[XmlArray("hints")]
		[XmlArrayItem("hint")]
		public List<string> Hints { get; set; }

		[XmlElement("solution")]
		public Label SolutionLabel { get; set; }

		[XmlElement("remove")]
		public Label[] RemovedLabels { get; set; }

		[XmlElement("comment")]
		public string CommentAfterExerciseIsSolved { get; set; }

		[XmlElement("expected")]
		// Ожидаемый корректный вывод программы
		public string ExpectedOutput { get; set; }

		[XmlElement("hide-expected-output")]
		public bool HideExpectedOutputOnError { get; set; }

		[XmlElement("validator")]
		public string ValidatorName { get; set; }

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			FillProperties(settings, lesson);
			RemovedLabels = RemovedLabels ?? new Label[0];
			var code = fs.GetContent(File);
			var regionRemover = new RegionRemover(LangId);

			var prelude = "";
			var preludeFile = settings.GetPrelude(LangId);
			if (preludeFile != null)
				prelude = fs.GetContent(Path.Combine("..", preludeFile));

			var exerciseCode = code;
			if (LangId == "cs" && prelude != "")
				exerciseCode = CsMembersRemover.RemoveUsings(code);

			IEnumerable<Label> notRemoved;
			exerciseCode = regionRemover.Remove(exerciseCode, RemovedLabels, out notRemoved);
			int index;
			exerciseCode = regionRemover.RemoveSolution(exerciseCode, SolutionLabel, out index);
			index += prelude.Length;

			ExerciseInitialCode = ExerciseInitialCode.RemoveCommonNesting();
			ExerciseCode = prelude + exerciseCode;
			IndexToInsertSolution = index;
			EthalonSolution = new RegionsExtractor(code, LangId).GetRegion(SolutionLabel);
			ValidatorName = string.Join(" ", LangId, ValidatorName);

			yield return this;
		}

		// То, что будет выполняться для проверки задания
		public string ExerciseCode { get; set; }
		// Индекс внутри ExerciseCode, куда нужно вставить код пользователя.
		public int IndexToInsertSolution { get; set; }
		// Если это вставить в ExerciseCode по индексу IndexToInsertSolution и выполнить полученный код, он должен вывести ExpectedOutput
		public string EthalonSolution { get; set; }

		public List<string> HintsMd
		{
			get { return Hints = Hints ?? new List<string>(); }
			set { Hints = value; }
		}

		[XmlIgnore]
		public SolutionBuilder Solution
		{
			get { return new SolutionBuilder(IndexToInsertSolution, ExerciseCode, ValidatorName); }
		}

		#region equals

		protected bool Equals(ExerciseBlock other)
		{
			return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsMd, other.HintsMd);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
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

		#endregion

		public override string ToString()
		{
			return string.Format("Exercise: {0}, Hints: {1}", ExerciseInitialCode, HintsMd);
		}
	}
}
