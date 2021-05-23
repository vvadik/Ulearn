using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Ulearn.Common.Extensions;

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
		
		[DataMember]
		[CanBeNull]
		public string[] Logs { get; set; }

		[DataMember]
		public int? TestNumber { get; set; }
		
		[DataMember] 
		[CanBeNull]
		public string Answer { get; set; }
		
		[DataMember]
		[CanBeNull]
		public string Test { get; set; }
		
		[DataMember]
		[CanBeNull] 
		public string StudentAnswer { get; set; }

		[IgnoreDataMember]
		private readonly int? timeLimit;

		public RunningResults()
		{
		}

		public RunningResults(string id, Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "")
		{
			Id = id;
			Verdict = verdict;
			CompilationOutput = compilationOutput;
			Output = output;
			Error = error;
			this.timeLimit = timeLimit;
		}

		public RunningResults(Verdict verdict, int? timeLimit = null, string compilationOutput = "", string output = "", string error = "")
			: this(null, verdict, timeLimit, compilationOutput, output, error)
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

		public string GetOutput(bool withFullDescription = false)
		{
			var output = string.Join("\n", new[] { Output, Error }.Where(s => !string.IsNullOrWhiteSpace(s)));

			switch (Verdict)
			{
				case Verdict.Ok:
					return output;
				case Verdict.TimeLimit:
					return output + TestNumberOutput 
								  + "\nВаше решение не успело пройти" 
								  + (TestNumber == null ? " все тесты" : " тест") 
								  + (timeLimit == null ? null : $" за {timeLimit} " + timeLimit.Value.SelectPluralWordInRussian(RussianPluralizationOptions.Seconds));
				case Verdict.WrongAnswer:
					return output + TestNumberOutput + WrongAnswerOutput(withFullDescription);
				case Verdict.RuntimeError:
					return output + TestNumberOutput + "\nПрограмма завершилась с ошибкой";
				default:
					return output + TestNumberOutput + "\n" + Verdict;
			}
		}

		public string GetLogs()
		{
			return Logs != null 
				? string.Join("\n", Logs) 
				: "";
		}

		private string TestNumberOutput => TestNumber == null ? null : $" \nТест №{TestNumber}";

		private string WrongAnswerOutput(bool withFullDescription)
		{
			var startOutput = TestNumberOutput +  "\nНеправильный ответ\n";
			if (withFullDescription)
			{
				return $"{startOutput}\n" +
						$"Входные данные:\n" +
						$"{Truncate(Test, 100)}\n\n" +
						$"Ожидаемый результат:\n" +
						$"{Truncate(Answer, 100)}\n\n" +
						$"Ваш результат:\n" +
						$"{Truncate(StudentAnswer, 100)}";
			}

			return startOutput;
		}

		private string Truncate(string source, int limit)
			=> source.Length <= limit
				? source
				: $"{source.Take(limit)}...";
	}
}