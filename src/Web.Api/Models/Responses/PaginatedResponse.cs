using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses
{
	[DataContract]
	public class PaginatedResponse : ApiResponse
	{
		[DataMember(Name = "$pagination")]
		public PaginationResponse PaginationResponse { get; set; }
	}

	[DataContract]
	public class PaginationResponse
	{
		[DataMember(Name = "offset")]
		public int Offset { get; set; }
		
		[DataMember(Name = "count")]
		public int Count { get; set; }
		
		[DataMember(Name = "total_count", EmitDefaultValue = true, Order = 1)]
		public int? TotalCount { get; set; }
	}
}