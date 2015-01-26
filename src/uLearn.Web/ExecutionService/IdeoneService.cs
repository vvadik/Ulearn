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

		public override bool IsSuccess
		{
			get { return result.Result == SubmitionResult.Success; }
		}

		public override bool IsCompilationError
		{
			get { return result.Result == SubmitionResult.CompilationError; }
		}

		public override bool IsTimeLimit
		{
			get { return result.Time > TimeSpan.FromSeconds(4); }
		}

		public override string CompilationErrorMessage
		{
			get
			{
				if (IsCompilationError)
					return !string.IsNullOrWhiteSpace(result.CompilationError) ? result.CompilationError : "Compilation Error";
				return "";
			}
		}

		public override bool IsInternalError
		{
			get { return result.Result == SubmitionResult.InternalError; }
		}

		public override string StdOut
		{
			get { return result.Output; }
		}

		public override string StdErr
		{
			get { return result.StdErr; }
		}

		protected override string Verdict
		{
			get { return result.Result.ToString(); }
		}
	}
}