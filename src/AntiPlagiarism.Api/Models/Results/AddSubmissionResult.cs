using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class AddSubmissionResult : ApiSuccessResult
	{
		[DataMember(Name = "submission_id")]
		public int SubmissionId { get; set; }
	}
}