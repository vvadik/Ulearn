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
			var error = Validator.FindSyntaxError(solution) ?? Validator.FindValidatorError(usersExercise, solution);
			return error == null ? SolutionBuildResult.Success(solution) : SolutionBuildResult.Error(error, solution);
		}
	}
}
