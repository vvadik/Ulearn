using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class AddCodeApiResult : ApiSuccessResult
	{
		[DataMember(Name = "submission_id")]
		public int SubmissionId { get; set; }
	}
}