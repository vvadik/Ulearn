using System.Threading.Tasks;

namespace uLearn.Web.ExecutionService
{
	public class RedundantExecutionService : IExecutionService
	{
		private readonly IExecutionService[] services;

		public RedundantExecutionService(params IExecutionService[] services)
		{
			this.services = services;
		}

		public async Task<SubmissionResult> Submit(string code, string displayName = null)
		{
			foreach (var executionService in services)
			{
				var res = await executionService.Submit(code, displayName);
				if (res != null)
					return res;
			}
			return null;
		}

		public string Name {
			get { return "All engines"; } 
		}

		public static RedundantExecutionService Default {
			get { return new RedundantExecutionService(new CsSandboxService(), new IdeoneService()); }
		}
	}
}