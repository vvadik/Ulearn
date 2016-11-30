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
			return $"ErrorMessage: {ErrorMessage ?? StyleMessage}, SourceCode:\r\n{SourceCode}";
		}

		public bool HasErrors => ErrorMessage != null;
		public bool HasStyleIssues => StyleMessage != null;

		public readonly string ErrorMessage;
		public readonly string StyleMessage;
		public readonly string SourceCode;
	}
}