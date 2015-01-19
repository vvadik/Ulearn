using uLearn;

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
			return details.CompilationInfo ?? "";
		}

		public static string GetOutput(this PublicSubmissionDetails details)
		{
			var output = details.Output;
			if (!string.IsNullOrEmpty(details.Error)) output += "\n" + details.Error;
			if (!details.IsSuccess())
			{
				if (details.IsTimeLimit())
					output += "\n Time limit exceeded";
				else
					output += "\n" + details.Verdict;
			}
			return NormalizeString(output);
		}

		public static string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}
	}
}