using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using RunCsJob.Api;
using uLearn.Extensions;

namespace uLearn.Model.Blocks
{
	[XmlType("single-file-exercise")]
	public class SingleFileExerciseBlock : ExerciseBlock
	{
		[XmlElement("solution")]
		public Label SolutionLabel { get; set; }

		[XmlElement("remove")]
		public Label[] RemovedLabels { get; set; }

		[XmlElement("prelude")]
		public string PreludeFile { get; set; }

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			FillProperties(context);
			RemovedLabels = RemovedLabels ?? new Label[0];
			if (PreludeFile == null)
			{
				PreludeFile = context.CourseSettings.GetPrelude(LangId);
				if (PreludeFile != null)
					PreludeFile = Path.Combine("..", PreludeFile);
			}

			var code = context.Dir.GetContent(File);
			var regionRemover = new RegionRemover(LangId);
			var extractor = context.GetExtractor(File, LangId, code);

			var prelude = "";
			if (PreludeFile != null)
				prelude = context.Dir.GetContent(PreludeFile);

			var exerciseCode = regionRemover.Prepare(code);
			IEnumerable<Label> notRemoved;
			exerciseCode = regionRemover.Remove(exerciseCode, RemovedLabels, out notRemoved);
			int index;
			exerciseCode = regionRemover.RemoveSolution(exerciseCode, SolutionLabel, out index);
			index += prelude.Length;

			ExerciseInitialCode = ExerciseInitialCode.RemoveCommonNesting();
			ExerciseCode = prelude + exerciseCode;
			IndexToInsertSolution = index;
			EthalonSolution = extractor.GetRegion(SolutionLabel);
			ValidatorName = string.Join(" ", LangId, ValidatorName);

			CheckScoringGroup(context.SlideTitle, context.CourseSettings.Scoring);

			yield return this;
		}

		// То, что будет выполняться для проверки задания
		public string ExerciseCode { get; set; }
		// Индекс внутри ExerciseCode, куда нужно вставить код пользователя.
		public int IndexToInsertSolution { get; set; }
		// Если это вставить в ExerciseCode по индексу IndexToInsertSolution и выполнить полученный код, он должен вывести ExpectedOutput
		public string EthalonSolution { get; set; }

		public override string GetSourceCode(string code)
		{
			return BuildSolution(code).SourceCode;
		}

		public override SolutionBuildResult BuildSolution(string userWrittenCode)
		{
			return new SolutionBuilder(IndexToInsertSolution, ExerciseCode, ValidatorName).BuildSolution(userWrittenCode);
		}

		public override RunnerSubmission CreateSubmition(string submissionId, string code)
		{
			return new FileRunnerSubmission
			{
				Id = submissionId,
				Code = GetSourceCode(code),
				Input = "",
				NeedRun = true
			};
		}
	}
}