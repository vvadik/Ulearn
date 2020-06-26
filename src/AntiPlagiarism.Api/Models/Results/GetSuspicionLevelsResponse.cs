using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class GetSuspicionLevelsResponse : SuccessResponse
	{
		[DataMember(Name = "suspicion_levels")]
		public SuspicionLevels SuspicionLevels { get; set; }
	}
}