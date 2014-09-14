namespace uLearn
{
	public class SolutionBuilder
	{
		public int IndexForInsert;
		public string ExerciseCode;
		public ISolutionValidator Validator;
		
		public SolutionBuildResult BuildSolution(string usersExercise)
		{
			var solution =  ExerciseCode.Insert(IndexForInsert, usersExercise + "\r\n");
			string message;
			if ((message = Validator.FindSyntaxError(solution)) != null)
				return SolutionBuildResult.Error(message, solution);
			if ((message = Validator.FindValidatorError(usersExercise, solution)) != null)
				return SolutionBuildResult.StyleIssue(message, solution);
			return SolutionBuildResult.Success(solution);
		}
	}
}
