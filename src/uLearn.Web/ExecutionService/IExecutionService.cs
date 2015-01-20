namespace uLearn.Web.ExecutionService
{
	public interface IExecutionService
	{
		SubmissionResult Submit(string code, string humanName = null);
	}
}