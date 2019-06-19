using System.Linq;

namespace Ulearn.Core.RunCheckerJobApi
{
	public class RunningResults
	{
		// TODO что куда пишется в каком случае?
		public string Id { get; set; }

		public Verdict Verdict { get; set; }

		public string CompilationOutput { get; set; }

		public string Output { get; set; }

		public string Error { get; set; }

		public RunningResults()
		{
			/* It's need to be here for model unpacking in API queries */
		}

		public RunningResults(string id, Verdict verdict, string compilationOutput = "", string output = "", string error = "")
		{
			Id = id;
			Verdict = verdict;
			CompilationOutput = compilationOutput;
			Output = output;
			Error = error;
		}

		public RunningResults(Verdict verdict, string compilationOutput = "", string output = "", string error = "")
		{
			Verdict = verdict;
			CompilationOutput = compilationOutput;
			Output = output;
			Error = error;
		}

		public override string ToString()
		{
			string message;
			if (Verdict == Verdict.CompilationError)
				message = CompilationOutput;
			else if (Verdict == Verdict.SandboxError)
				message = Error;
			else
				message = string.Join("\n", new[] { Output, Error }.Where(s => !string.IsNullOrWhiteSpace(s)));
			return $"Id: {Id}, Verdict: {Verdict}: {message}";
		}

		public string GetOutput()
		{
			var output = string.Join("\n", new[] { Output, Error }.Where(s => !string.IsNullOrWhiteSpace(s)));

			switch (Verdict)
			{
				case Verdict.Ok:
					return output;
				case Verdict.TimeLimit:
					return output + "\n Ваше решение не успело пройти все тесты за 10 секунд";
				default:
					return output + "\n" + Verdict;
			}
		}
	}
}