namespace uLearn
{
	public class SolutionBuildResult
	{
		public static SolutionBuildResult Error(string errorMessage) {  return new SolutionBuildResult(errorMessage, null);}
		public static SolutionBuildResult Success(string sourceCode) { return new SolutionBuildResult(null, sourceCode); }

		private SolutionBuildResult(string errorMessage, string sourceCode)
		{
			ErrorMessage = errorMessage;
			SourceCode = sourceCode;
		}
		public bool HasErrors { get { return ErrorMessage != null; } }
		public readonly string ErrorMessage;
		public readonly string SourceCode;
	}
}