using System;
using System.Threading.Tasks;
using Job;

namespace uLearn.Web.ExecutionService
{
	public partial class CsSandboxExecutionService : IExecutionService
	{
		private static readonly string Token;
		private static readonly string Address;

		public CsSandboxExecutionService()
			: this(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
		{
		}

		public CsSandboxExecutionService(TimeSpan httpTimeout, TimeSpan executionTimeout)
		{
//			client = new CsSandboxClient(httpTimeout, Token, Address, (int)executionTimeout.TotalSeconds);
		}

//		private readonly CsSandboxClient client;

		public async Task<SubmissionResult> Submit(string code, string displayName = null)
		{
//			try
//			{
//				var details = await client.Submit(code, "", displayName);
//				return details == null ? null : new CsSandboxSubmissionResult(details);
//			}
//			catch (TaskCanceledException)
//			{
//				return null;
//			}
//			catch (CsSandboxClientException)
//			{
//				return null;
//			}
			return null;
		}

		public string Name
		{
			get { return "CsSandbox"; }
		}
	}

	public class CsSandboxSubmissionResult : SubmissionResult
	{
		private readonly PublicSubmissionDetails details;

		public CsSandboxSubmissionResult(PublicSubmissionDetails details)
		{
			this.details = details;
		}

		public override bool IsSuccess
		{
//			get { return details.Verdict == CsSandboxApi.Verdict.Ok; }
			get { return true; }
		}

		public override bool IsCompilationError
		{
//			get { return details.Verdict == CsSandboxApi.Verdict.CompilationError; }
			get { return true; }
		}

		public override bool IsTimeLimit
		{
//			get { return details.Verdict == CsSandboxApi.Verdict.TimeLimit; }
			get { return true; }
		}

		public override string CompilationErrorMessage
		{
			get { return details.CompilationInfo ?? ""; }
		}

		public override bool IsInternalError
		{
//			get { return details.Verdict == CsSandboxApi.Verdict.SandboxError; }
			get { return true; }
		}

		public override string StdOut
		{
			get { return details.Output; }
		}

		public override string StdErr
		{
			get { return details.Error; }
		}

		protected override string Verdict
		{
			get { return details.Verdict.ToString(); }
		}

		public override string ServiceName
		{
			get { return "CsSandbox"; }
		}
	}
}