using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class ApiSuccessResult : ApiResult
	{
		protected ApiSuccessResult()
		{
			Status = ApiResultStatus.Ok;
		}
	}
}