using System.Threading.Tasks;

namespace uLearn.Web.ExecutionService
{
	public interface IExecutionService
	{
		Task<SubmissionResult> Submit(string code, string displayName = null);
		string Name { get; }
	}
}