using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class GetSubmissionPlagiarismsParameters: ApiParameters
	{
		[BindRequired]
		[FromQuery(Name = "submission_id")]
		public int SubmissionId { get; set; }
	}
}
