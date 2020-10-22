using System.Runtime.Serialization;
using Database.Models;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	public enum AutomaticExerciseCheckingProcessStatus
	{
		ServerError, // Ошибка на сервере или статус SandoxError в чеккере
		Done, // Проверена
		Waiting, // В очереди на проверку
		Running, // Проверяется
		WaitingTimeLimit, // Задача выкинута из очереди, потому что её никто не взял на проверку
	}

	public enum AutomaticExerciseCheckingResult
	{
		NotChecked,
		CompilationError,
		RightAnswer,
		WrongAnswer
	}

	[DataContract]
	public class ExerciseAutomaticCheckingResponse
	{
		[DataMember]
		public AutomaticExerciseCheckingProcessStatus ProcessStatus;

		[DataMember]
		public AutomaticExerciseCheckingResult Result;

		[CanBeNull]
		[DataMember]
		public string CompilationError;

		[CanBeNull]
		[DataMember]
		public string Output;

		[DataMember]
		public float? Points;

		public static ExerciseAutomaticCheckingResponse Build([NotNull] AutomaticExerciseChecking checking)
		{
			var result = new ExerciseAutomaticCheckingResponse
			{
				ProcessStatus = AutomaticExerciseCheckingProcessStatus.ServerError,
				Result = AutomaticExerciseCheckingResult.NotChecked
			};
			var processStatus = GetProcessStatus(checking);
			if (processStatus != AutomaticExerciseCheckingProcessStatus.Done)
				return result;
			if (checking.IsCompilationError)
			{
				result.Result = AutomaticExerciseCheckingResult.CompilationError;
				result.CompilationError = result.CompilationError;
				return result;
			}
			result.Result = checking.IsRightAnswer ? AutomaticExerciseCheckingResult.RightAnswer : AutomaticExerciseCheckingResult.WrongAnswer;
			result.Output = checking.Output?.Text;
			result.Points = checking.Points;
			return result;
		}

		private static AutomaticExerciseCheckingProcessStatus GetProcessStatus(AutomaticExerciseChecking checking)
		{
			return checking.Status switch
			{
				AutomaticExerciseCheckingStatus.Done => AutomaticExerciseCheckingProcessStatus.Done,
				AutomaticExerciseCheckingStatus.Waiting => AutomaticExerciseCheckingProcessStatus.Waiting,
				AutomaticExerciseCheckingStatus.Running => AutomaticExerciseCheckingProcessStatus.Running,
				AutomaticExerciseCheckingStatus.RequestTimeLimit => AutomaticExerciseCheckingProcessStatus.WaitingTimeLimit,
				AutomaticExerciseCheckingStatus.Error => AutomaticExerciseCheckingProcessStatus.ServerError,
				_ => AutomaticExerciseCheckingProcessStatus.ServerError
			};
		}
	}
}