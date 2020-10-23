using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	public enum SolutionRunStatus
	{
		Success,
		InternalServerError, // Ошибка в проверяющей системе, подробности могут быть в Message. Если submission создан, он лежит в Submission, иначе null.
		Ignored, // Сервер отказался обрабатывать решение, причина написана в Message. Cлишком частые запросы на проверку или слишком длинный код.
		SubmissionCheckingTimeout, // Ждали, но не дожадлись проверки
		// В случае ошибки компиляции Submission создается не всегда. Не создается для C#-задач. Тогда текст ошибки компиляции кладется в Message.
		// Если Submission создался, то об ошибках компиляции пишется внутри Submission -> AutomaticChecking.
		CompileError 
	}

	[DataContract]
	public class RunSolutionResponse
	{
		[DataMember]
		public SolutionRunStatus SolutionRunStatus;

		[CanBeNull]
		[DataMember]
		public string Message { get; set; } // Сообщение от проверяющей системы в случае ошибок на сервере и в случае некоторых ошибок компиляции.

		[CanBeNull]
		[DataMember]
		public SubmissionInfo Submission; // Если submission создан, он лежит в Submission, иначе null. Не создан в случае некоторых ошибок на сервере и иногда в случае ошибок компиляции.

		[DataMember]
		public int? Score { get; set; } // Если null, то не изменился. Если не null, то не обязательно изменился.

		[DataMember]
		public bool? WaitingForManualChecking { get; set; } // Если null, то не изменился. Если не null, то не обязательно изменился.

		public RunSolutionResponse(SolutionRunStatus status)
		{
			SolutionRunStatus = status;
		}
	}
}