namespace uLearn
{
	public class SolutionBuildResult
	{
		public static SolutionBuildResult Error(string errorMessage, string sourceCode) {  return new SolutionBuildResult(errorMessage, sourceCode);}
		public static SolutionBuildResult Success(string sourceCode) { return new SolutionBuildResult(null, sourceCode); }

		private SolutionBuildResult(string errorMessage, string sourceCode)
		{
			ErrorMessage = errorMessage;
			SourceCode = sourceCode;
		}

		public override string ToString()
		{
			return string.Format("ErrorMessage: {0}, SourceCode:\r\n{1}", ErrorMessage, SourceCode);
		}

		public bool HasErrors { get { return ErrorMessage != null; } }
		public readonly string ErrorMessage;
		public readonly string SourceCode;
	}
}