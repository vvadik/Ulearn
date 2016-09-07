namespace uLearn.Web.Models
{
	public class RunSolutionResult
	{
		public bool IsCompillerFailure;
		public bool IsCompileError;
		public bool IsStyleViolation;
		public bool IsRightAnswer;
		public string ExpectedOutput { get; set; }
		public string ActualOutput { get; set; }
		public string CompilationError { get; set; }
		public string ExecutionServiceName { get; set; }
		public bool SentToReview { get; set; }
	}
}