using System;
using System.Linq;

namespace uLearn.Web.ExecutionService
{
	public abstract class SubmissionResult
	{
		public abstract bool IsSuccess { get; }
		public abstract bool IsCompilationError { get; }
		public abstract bool IsTimeLimit { get; }
		public abstract string CompilationErrorMessage { get; }
		public abstract bool IsInternalError { get; }
		public abstract string StdOut { get; }
		public abstract string StdErr { get; }
		protected abstract string Verdict { get; }
		public abstract string ServiceName { get; }

		public string GetOutput()
		{
			var output = StdOut;
			if (!string.IsNullOrEmpty(StdErr)) output += "\n" + StdErr;
			if (!IsSuccess)
			{
				if (IsTimeLimit)
					output += "\n Time limit exceeded";
				else
					output += "\n" + Verdict;
			}
			return output.NormalizeEoln();
		}
	}
}