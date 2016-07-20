namespace uLearn
{
    internal static class ValidatorExtention
    {
        public static SolutionBuildResult ValidateSolution(this ISolutionValidator validator, string code)
        {
            string message;

            if ((message = validator.FindSyntaxError(code)) != null)
                return SolutionBuildResult.Error(message, code);
            if ((message = validator.FindValidatorError(code, code)) != null)
                return SolutionBuildResult.StyleIssue(message, code);
            return SolutionBuildResult.Success(code);
        }

        public static SolutionBuildResult ValidateFileSolution(this ISolutionValidator validator, string code)
        {
            string message;
            if ((message = validator.FindFullSourceError(code)) != null)
                return SolutionBuildResult.Error(message, code);
            return validator.ValidateSolution(code);
        }
    }
}