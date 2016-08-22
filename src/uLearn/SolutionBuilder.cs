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
			return Validator.ValidateFileSolution(solution);
		}
	}
}