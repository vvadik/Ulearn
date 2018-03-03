namespace uLearn
{
	internal static class ValidatorExtension
	{
		public static SolutionBuildResult ValidateSolution(this ISolutionValidator validator, string userWrittenCode, string fullCodeFile)
		{
			string message;

			if ((message = validator.FindSyntaxError(fullCodeFile)) != null ||
				(message = validator.FindStrictValidatorErrors(userWrittenCode, fullCodeFile)) != null)
			{
				return SolutionBuildResult.Error(message, fullCodeFile);
			}

			if ((message = validator.FindValidatorErrors(userWrittenCode, fullCodeFile)) != null)
			{
				return SolutionBuildResult.StyleIssue(message, fullCodeFile);
			}

			return SolutionBuildResult.Success(fullCodeFile);
		}

		public static SolutionBuildResult ValidateSingleFileSolution(this ISolutionValidator validator, string userWrittenCode, string fullCodeFile)
		{
			string message;
			if ((message = validator.FindFullSourceError(userWrittenCode)) != null)
				return SolutionBuildResult.Error(message, fullCodeFile);
			return validator.ValidateSolution(userWrittenCode, fullCodeFile);
		}
	}
}