using System.Linq;
using System.Runtime.Serialization;
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

		[JsonConstructor]
		public RunningResults(string id, Verdict verdict, string compilationOutput = "", string output = "", string error = "", float? points = null)
		{
			Id = id;
			Verdict = verdict;
			CompilationOutput = compilationOutput ?? "";
			Output = output ?? "";
			Error = error ?? "";
			Points = points;
		}

		public RunningResults(Verdict verdict, string compilationOutput = "", string output = "", string error = "", float? points = null)
		{
			Verdict = verdict;
			CompilationOutput = compilationOutput ?? "";
			Output = output ?? "";
			Error = error ?? "";
			Points = points;
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
					return output + "\n Ваше решение не успело пройти все тесты за 10 секунд"; // TODO: убрать константу 10.
				default:
					return output + "\n" + Verdict;
			}
		}
	}
}