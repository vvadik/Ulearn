 // ReSharper disable once CheckNamespace
namespace RunCsJob.Api
{
	public class RunnerSubmition
	{
		public string Id;
		public string Code;
		public string Input;
		public bool NeedRun;

		public override string ToString()
		{
			return string.Format("Id: {0}, NeedRun: {1}", Id, NeedRun);
		}
	}

	public enum Verdict
	{
		NA = 0,
		Ok = 1,
		CompilationError = 2,
		RuntimeError = 3,
		SecurityException = 4,
		SandboxError = 5,
		OutputLimit = 6,
		TimeLimit = 7,
		MemoryLimit = 8,
	}

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
				Verdict == Verdict.SandboxError ? Error : Output);
		}

		public string GetOutput()
		{
			var output = Output;
			if (!string.IsNullOrEmpty(Error)) output += "\n" + Error;
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
