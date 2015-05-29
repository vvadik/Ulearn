using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using uLearn.CSharp;

namespace uLearn.Model.Blocks
{
	[XmlType("execirse")]
	public class ExecirseConfigBlock : IncludeCode
	{
		[XmlElement("inital-code")]
		public string InitalCode { get; set; }

		[XmlArray("hints")]
		[XmlArrayItem("hint")]
		public string[] Hints { get; set; }

		[XmlElement("solution")]
		public Label Solution { get; set; }

		[XmlElement("remove")]
		public Label[] RemovedLabels { get; set; }

		[XmlElement("comment")]
		public string Comment { get; set; }

		[XmlElement("expected")]
		public string ExpectedOutput { get; set; }

		[XmlElement("hide-expected-output")]
		public bool HideExpectedOutputOnError { get; set; }
		
		[XmlElement("validator")]
		public string Validator { get; set; }

		public override IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			FillProperties(settings, lesson);
			RemovedLabels = RemovedLabels ?? new Label[0];
			var code = fs.GetContent(File);
			var ethalonSolution = new RegionsExtractor(code, LangId).GetRegion(Solution);
			var regionRemover = new RegionRemover(LangId);

			var prelude = "";
			var preludeFile = settings.GetPrelude(LangId);
			if (preludeFile != null)
				prelude = fs.GetContent(Path.Combine("..", preludeFile));

			var exerciseCode = code;
			if (LangId == "cs" && prelude != "")
				exerciseCode = CsMembersRemover.RemoveUsings(exerciseCode);

			regionRemover.Remove(ref exerciseCode, RemovedLabels);
			var index = regionRemover.RemoveSolution(ref exerciseCode, Solution) + prelude.Length;
			
			yield return new ExerciseBlock
			{
				CommentAfterExerciseIsSolved = Comment,
				EthalonSolution = ethalonSolution,
				ExerciseCode = prelude + exerciseCode,
				ExerciseInitialCode = InitalCode.RemoveCommonNesting(),
				ExpectedOutput = ExpectedOutput,
				HideExpectedOutputOnError = HideExpectedOutputOnError,
				HintsMd = Hints.ToList(),
				IndexToInsertSolution = index,
				Lang = LangId,
				LangVer = LangVer,
				ValidatorName = string.Join(" ", LangId, Validator)
			};
		}
	}
}