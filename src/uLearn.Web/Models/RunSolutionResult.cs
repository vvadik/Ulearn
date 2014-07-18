namespace uLearn.Web.Models
{
	public class RunSolutionResult
	{
		public bool IsRightAnswer;
		public string ExpectedOutput { get; set; }
		public string ActualOutput { get; set; }
		public string CompilationError { get; set; }
	}
}