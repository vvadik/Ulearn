using System.Threading.Tasks;

namespace uLearn.Web.ExecutionService
{
	public interface IExecutionService
	{
		Task<SubmissionResult> Submit(string code, string humanName = null);
		string Name { get; }
	}
}