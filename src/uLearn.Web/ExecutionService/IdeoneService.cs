using System;
using uLearn.Web.Ideone;

namespace uLearn.Web.ExecutionService
{
	public class IdeoneService : IExecutionService
	{
		private readonly Ideone.ExecutionService executionService = new Ideone.ExecutionService();

		public SubmissionResult Submit(string code, string humanName = null)
		{
			var result = executionService.Submit(code, "").Result;
			return new IdeoneSubmissionResult(result);
		}
	}

	public class IdeoneSubmissionResult : SubmissionResult
	{
		private readonly GetSubmitionDetailsResult result;

		public IdeoneSubmissionResult(GetSubmitionDetailsResult result)
		{
			this.result = result;
		}

		public override bool IsSuccess()
		{
			return result.Result == SubmitionResult.Success;
		}

		public override bool IsCompilationError()
		{
			return result.Result == SubmitionResult.CompilationError;
		}

		public override bool IsTimeLimit()
		{
			return result.Time > TimeSpan.FromSeconds(4);
		}

		public override string GetCompilationError()
		{
			if (IsCompilationError())
				return !string.IsNullOrWhiteSpace(result.CompilationError) ? result.CompilationError : "Compilation Error";
			return "";
		}

		public override bool IsInternalError()
		{
			return result.Result == SubmitionResult.InternalError;
		}
	}
}