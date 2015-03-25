using System;
using System.Threading.Tasks;
using CsSandboxApi;

namespace uLearn.Web.ExecutionService
{
	public partial class CsSandboxService : IExecutionService
	{
		private static readonly string Token;
		private static readonly string Address;

		private readonly CsSandboxClient client = new CsSandboxClient(TimeSpan.FromSeconds(1), Token, Address);

		public async Task<SubmissionResult> Submit(string code, string displayName = null)
		{
			try
			{
				var details = await client.Submit(code, "", displayName);
				return details == null ? null : new CsSandboxSubmissionResult(details);
			}
			catch (CsSandboxClientException)
			{
				return null;
			}
		}

		public string Name {
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
			get { return details.Verdict == CsSandboxApi.Verdict.Ok; }
		}

		public override bool IsCompilationError
		{
			get { return details.Verdict == CsSandboxApi.Verdict.CompilationError; }
		}

		public override bool IsTimeLimit
		{
			get { return details.Verdict == CsSandboxApi.Verdict.TimeLimit; }
		}

		public override string CompilationErrorMessage
		{
			get { return details.CompilationInfo ?? ""; }
		}

		public override bool IsInternalError
		{
			get { return details.Verdict == CsSandboxApi.Verdict.SandboxError; }
		}

		public override string StdOut
		{
			get { return details.Output; }
		}

		public override string StdErr {
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