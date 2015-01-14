namespace CsSandboxApi
{
	public static class SubmissionDetailsExtensions
	{
		public static bool IsSuccess(this PublicSubmissionDetails details)
		{
			return details.Verdict == Verdict.Ok;
		}

		public static bool IsCompilationError(this PublicSubmissionDetails details)
		{
			return details.Verdict == Verdict.CompilationError;
		}

		public static bool IsTimeLimit(this PublicSubmissionDetails details)
		{
			return details.Verdict == Verdict.TimeLimit;
		}

		public static string GetCompilationError(this PublicSubmissionDetails details)
		{
			if (!details.IsCompilationError() || string.IsNullOrEmpty(details.CompilationInfo))
				return "";
			return details.CompilationInfo;
		}
	}
}