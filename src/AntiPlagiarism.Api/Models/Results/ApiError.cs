using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class ApiError : ApiResult
	{
		[DataMember(Name = "error")]
		public string Error { get; set; }

		public static ApiError Create(string error)
		{
			return new ApiError
			{
				Status = ApiResultStatus.Error,
				Error = error,
			};
		}
	}
}