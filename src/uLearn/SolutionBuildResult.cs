namespace uLearn
{
	public class SolutionBuildResult
	{
		public static SolutionBuildResult StyleIssue(string errorMessage, string sourceCode) { return new SolutionBuildResult(sourceCode, null, errorMessage); }
		public static SolutionBuildResult Error(string errorMessage, string sourceCode) { return new SolutionBuildResult(sourceCode, errorMessage); }
		public static SolutionBuildResult Success(string sourceCode) { return new SolutionBuildResult(sourceCode); }

		private SolutionBuildResult(string sourceCode, string errorMessage = null, string styleMessage = null)
		{
			ErrorMessage = errorMessage;
			SourceCode = sourceCode;
			StyleMessage = styleMessage;
		}

		public override string ToString()
		{
			return string.Format("ErrorMessage: {0}, SourceCode:\r\n{1}", ErrorMessage ?? StyleMessage, SourceCode);
		}

		public bool HasErrors { get { return ErrorMessage != null; } }
		public bool HasStyleIssues { get { return StyleMessage != null; } }

		public readonly string ErrorMessage;
		public readonly string StyleMessage;
		public readonly string SourceCode;
	}
}