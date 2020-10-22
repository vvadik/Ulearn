using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	[DataContract]
	public class RunSolutionResponse
	{
		[DataMember]
		public bool Ignored { get; set; }  // Сервер отказался обрабатывать решение, причина написана в Message. Cлишком частые запросы на проверку или слишком длинный код.

		[DataMember]
		public bool IsInternalServerError; // Ошибка в проверяющей системе, подробности могут быть в Message. Если submission создан, он лежит в Submission, иначе null.

		[DataMember]
		// В случае ошибки компиляции Submission создается не всегда. Не создается для C#-задач. Тогда текст ошибки компиляции кладется в Message.
		// Если Submission создался, то об ошибках компиляции пишется внутри Submission -> AutomaticChecking.
		public bool IsCompileError; 

		[CanBeNull]
		[DataMember]
		public string Message { get; set; } // Сообщение от проверяющей системы в случае ошибок на сервере и в случае некоторых ошибок компиляции.

		[CanBeNull]
		[DataMember]
		public SubmissionInfo Submission; // Если submission создан, он лежит в Submission, иначе null. Не создан в случае некоторых ошибок на сервере и иногда в случае ошибок компиляции.

		[DataMember]
		public int? Score { get; set; } // Если null, то не изменился. Если не null, то не обязательно изменился.
	}
}