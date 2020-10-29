using System.Collections.Generic;
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
		WaitingTimeLimitExceeded // Задача выкинута из очереди, потому что её никто не взял на проверку
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

		[NotNull]
		[DataMember]
		public List<ReviewInfo> Reviews;

		public static ExerciseAutomaticCheckingResponse Build([NotNull] AutomaticExerciseChecking checking, [NotNull]List<ReviewInfo> botReviews)
		{
			var result = new ExerciseAutomaticCheckingResponse
			{
				ProcessStatus = AutomaticExerciseCheckingProcessStatus.ServerError,
				Result = AutomaticExerciseCheckingResult.NotChecked
			};
			result.ProcessStatus = GetProcessStatus(checking);
			if (result.ProcessStatus != AutomaticExerciseCheckingProcessStatus.Done)
				return result;
			if (checking.IsCompilationError)
			{
				result.Result = AutomaticExerciseCheckingResult.CompilationError;
				result.CompilationError = checking.CompilationError?.Text;
				return result;
			}
			result.Result = checking.IsRightAnswer ? AutomaticExerciseCheckingResult.RightAnswer : AutomaticExerciseCheckingResult.WrongAnswer;
			result.Output = checking.Output?.Text;
			result.Points = checking.Points;
			result.Reviews = botReviews;
			return result;
		}

		private static AutomaticExerciseCheckingProcessStatus GetProcessStatus(AutomaticExerciseChecking checking)
		{
			return checking.Status switch
			{
				AutomaticExerciseCheckingStatus.Done => AutomaticExerciseCheckingProcessStatus.Done,
				AutomaticExerciseCheckingStatus.Waiting => AutomaticExerciseCheckingProcessStatus.Waiting,
				AutomaticExerciseCheckingStatus.Running => AutomaticExerciseCheckingProcessStatus.Running,
				AutomaticExerciseCheckingStatus.RequestTimeLimit => AutomaticExerciseCheckingProcessStatus.WaitingTimeLimitExceeded,
				AutomaticExerciseCheckingStatus.Error => AutomaticExerciseCheckingProcessStatus.ServerError,
				_ => AutomaticExerciseCheckingProcessStatus.ServerError
			};
		}
	}
}