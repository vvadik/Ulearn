using uLearn.CSharp;

namespace uLearn
{
	public class SolutionBuilder
	{
		public SolutionBuilder(int indexForInsert, string exerciseCode, string validatorName)
		{
			IndexForInsert = indexForInsert;
			ExerciseCode = exerciseCode;
			Validator = ValidatorsRepository.Get(validatorName);
		}

		private readonly int IndexForInsert;
		private readonly string ExerciseCode;
		private readonly ISolutionValidator Validator;
		
		public SolutionBuildResult BuildSolution(string usersExercise)
		{
			var solution = ExerciseCode.Insert(IndexForInsert, usersExercise + "\r\n");
			string message;
			if ((message = Validator.FindFullSourceError(usersExercise)) != null)
				return SolutionBuildResult.Error(message, usersExercise);
			if ((message = Validator.FindSyntaxError(solution)) != null)
				return SolutionBuildResult.Error(message, solution);
			if ((message = Validator.FindValidatorError(usersExercise, solution)) != null)
				return SolutionBuildResult.StyleIssue(message, solution);
			return SolutionBuildResult.Success(solution);
		}
	}
}
