namespace uLearn.Web.ExecutionService
{
	public abstract class SubmissionResult
	{
		public abstract bool IsSuccess();
		public abstract bool IsCompilationError();
		public abstract bool IsTimeLimit();
		public abstract string GetCompilationError();
		public abstract bool IsInternalError();

		public virtual string StdOut { get; private set; }
		public virtual string StdErr { get; private set; }
		protected virtual string Verdict { get; private set; }

		public string GetOutput()
		{
			var output = StdOut;
			if (!string.IsNullOrEmpty(StdErr)) output += "\n" + StdErr;
			if (!IsSuccess())
			{
				if (IsTimeLimit())
					output += "\n Time limit exceeded";
				else
					output += "\n" + Verdict;
			}
			return output.NormalizeEoln();
		}
	}
}