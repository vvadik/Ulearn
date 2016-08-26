using System;
using System.Globalization;
using System.Linq;

namespace RunCsJob.Api
{
	public class RunningResults
	{
		public string Id { get; set; }
		public string CompilationOutput { get; set; }
		public Verdict Verdict { get; set; }
		public string Output { get; set; }
		public string Error { get; set; }

		public override string ToString()
		{
			return string.Format("Id: {0}, Verdict: {1}: {2}", Id, Verdict,
				Verdict == Verdict.SandboxError ? Error : Verdict == Verdict.CompilationError ? CompilationOutput : Output);
		}

		public string GetOutput()
		{
			var output = Output;
			if (!string.IsNullOrEmpty(Error))
				output += "\n" + Error;
			if (Verdict != Verdict.Ok)
			{
				if (Verdict == Verdict.TimeLimit)
					output += "\n Time limit exceeded";
				else
					output += "\n" + Verdict;
			}
			return output;
		}
	}
}