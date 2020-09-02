using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Ulearn.Core.RunCheckerJobApi
{
	public class RunningResults
	{
		public string Id { get; set; }

		[JsonProperty(Required = Required.Always)]
		public readonly Verdict Verdict;

		[NotNull] public readonly string CompilationOutput;

		// Для вывода пользователю используется GetOutput()
		[NotNull] public readonly string Output; // Для C# это stdout
		[NotNull] public readonly string Error; // Для C# это stderr
		public readonly float? Points;
		public readonly int? TimeLimit;
		[CanBeNull] public List<StyleError> StyleErrors;

		[JsonConstructor]
		public RunningResults(string id, Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "", float? points = null, List<StyleError> styleErrors = null)
		{
			Id = id;
			Verdict = verdict;
			CompilationOutput = compilationOutput ?? "";
			Output = output ?? "";
			Error = error ?? "";
			Points = points;
			TimeLimit = timeLimit;
			StyleErrors = styleErrors;
		}

		public RunningResults(Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "", float? points = null, List<StyleError> styleErrors = null)
		{
			Verdict = verdict;
			CompilationOutput = compilationOutput ?? "";
			Output = output ?? "";
			Error = error ?? "";
			Points = points;
			TimeLimit = timeLimit;
			StyleErrors = styleErrors;
		}

		public override string ToString()
		{
			string message;
			switch (Verdict)
			{
				case Verdict.CompilationError:
					message = CompilationOutput;
					break;
				case Verdict.SandboxError:
					message = Error;
					break;
				default:
					message = string.Join("\n", new[] { Output, Error }.Where(s => !string.IsNullOrWhiteSpace(s)));
					break;
			}

			var pointsString = Points?.ToString("#.##"); // Печатает только значимые цифры, максимум две, округляет.
			return $"Id: {Id}, Verdict: {Verdict}: {message}" + (pointsString == null ? null : $", Points: {pointsString}");
		}

		public string GetOutput()
		{
			var output = string.Join("\n", new[] { Output, Error }.Where(s => !string.IsNullOrWhiteSpace(s)));

			switch (Verdict)
			{
				case Verdict.Ok:
					return output;
				case Verdict.TimeLimit:
					return output + "\n Ваше решение не успело пройти все тесты" + (TimeLimit == null ? null : $" за {TimeLimit} секунд"); // TODO: Окончание слова секунд сейчас рассчитано на числа, кратные 10.
				default:
					return output + "\n" + Verdict;
			}
		}
	}
}