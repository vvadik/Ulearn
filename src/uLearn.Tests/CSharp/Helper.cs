using System.Collections.Generic;
using System.Linq;
using test;

namespace uLearn.CSharp
{
	public static class Helper
	{
		public static readonly string[] WrongAnswerNames = {
			$"{nameof(AnotherTask)}.WrongAnswer.88.cs",
			$"{nameof(MeaningOfLifeTask)}.WrongAnswer.27.cs",
			$"{nameof(MeaningOfLifeTask)}.WrongAnswer.Type.cs",
			$"{nameof(MeaningOfLifeTask)}.WrongAnswer.21.plus.21.cs"
		};

		public static readonly string[] SolutionNames = {
			$"{nameof(AnotherTask)}.Solution.cs",
			$"{nameof(MeaningOfLifeTask)}.Solution.cs"
		};

		public static string OrderedWrongAnswersAndSolutionNames => ToString(WrongAnswersAndSolutionNames.OrderBy(n => n));

		public static IEnumerable<string> WrongAnswersAndSolutionNames => WrongAnswerNames.Concat(SolutionNames);

		public static string WrongAnswerNamesToString() => ToString(WrongAnswerNames);

		public static string SolutionNamesToString() => ToString(SolutionNames);

		public static string ToString(IEnumerable<string> arr) => string.Join(", ", arr);
	}
}