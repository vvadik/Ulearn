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
			return $"Id: {Id}, Verdict: {Verdict}: {(Verdict == Verdict.SandboxError ? Error : Verdict == Verdict.CompilationError ? CompilationOutput : Output)}";
		}

		public string GetOutput()
		{
			var output = Output;
			if (!string.IsNullOrEmpty(Error))
				output += "\n" + Error;

			switch (Verdict)
			{
				case Verdict.Ok:
					return output;
				case Verdict.TimeLimit:
					return output + "\n Time limit exceeded";
				default:
					return output + "\n" + Verdict;
			}
		}
	}
}