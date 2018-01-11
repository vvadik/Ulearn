using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public abstract class ApiResult
	{
		[DataMember(Name = "status")]
		[JsonConverter(typeof(StringEnumConverter), true)]
		public ApiResultStatus Status { get; set; }
	}
}