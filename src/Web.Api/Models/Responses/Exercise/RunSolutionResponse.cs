using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	[DataContract]
	public class RunSolutionResponse
	{
		[DataMember(Name = "ignored")]
		public bool Ignored { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "submissionId")]
		public int SubmissionId { get; set; }

		[DataMember(Name = "isCompileError")]
		public bool IsCompileError;

		[DataMember(Name = "isRightAnswer")]
		public bool IsRightAnswer;

		[DataMember(Name = "isInternalServerError")]
		public bool IsInternalServerError;

		[DataMember(Name = "expectedOutput")]
		public string ExpectedOutput { get; set; }

		[DataMember(Name = "actualOutput")]
		public string ActualOutput { get; set; }

		[DataMember(Name = "sentToReview")]
		public bool SentToReview { get; set; }

		[DataMember(Name = "executionServiceName")]
		public string ExecutionServiceName { get; set; }

		[DataMember(Name = "styleMessages")]
		public List<string> StyleMessages { get; set; }
	}
}