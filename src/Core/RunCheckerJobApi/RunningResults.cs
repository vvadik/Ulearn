using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Ulearn.Core.RunCheckerJobApi
{
	[DataContract]
	public class RunningResults
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public Verdict Verdict { get; set; }

		[IgnoreDataMember]
		private string compilationOutput;
		[DataMember]
		[NotNull]
		public string CompilationOutput
		{
			get => compilationOutput ?? "";
			set => compilationOutput = value;
		}

		[IgnoreDataMember]
		private string output;
		// Для вывода пользователю используется GetOutput()
		[DataMember]
		[NotNull]
		public string Output // Для C# это stdout
		{
			get => output ?? ""; 
			set => output = value;
		}

		[IgnoreDataMember]
		private string error;
		[DataMember]
		[NotNull]
		public string Error // Для C# это stderr
		{
			get => error ?? ""; // Для C# это stdout
			set => error = value;
		}

		[DataMember]
		public float? Points { get; set; }

		[DataMember]
		[CanBeNull]
		public List<StyleError> StyleErrors { get; set; }

		[IgnoreDataMember]
		private readonly int? timeLimit;

		public RunningResults()
		{
		}

		public int TestNumber;

		[JsonConstructor]
		public RunningResults(string id, Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "", float? points = null, List<StyleError> styleErrors = null)
		{
			Id = id;
			Verdict = verdict;
			CompilationOutput = compilationOutput;
			Output = output;
			Error = error;
			Points = points;
			this.timeLimit = timeLimit;
			StyleErrors = styleErrors;
		}

		public RunningResults(Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "", float? points = null, List<StyleError> styleErrors = null)
			: this(null, verdict, timeLimit, compilationOutput, output, error, points, styleErrors)
		{
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
					return output + "\n Ваше решение не успело пройти все тесты" + (timeLimit == null ? null : $" за {timeLimit} секунд"); // TODO: Окончание слова секунд сейчас рассчитано на числа, кратные 10.
				case Verdict.WrongAnswer:
					return output + "\n Неправильный ответ";
				case Verdict.RuntimeError:
					return output + "\n Ошибка времени выполнения";
				default:
					return output + "\n" + Verdict;
			}
		}
	}
}