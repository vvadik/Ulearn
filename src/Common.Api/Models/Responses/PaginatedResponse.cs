using System.Runtime.Serialization;

namespace Ulearn.Common.Api.Models.Responses
{
	[DataContract]
	public class PaginatedResponse : SuccessResponse
	{
		[DataMember(Name = "pagination", Order = 100)]
		public PaginationResponse PaginationResponse { get; set; }
	}

	[DataContract]
	public class PaginationResponse
	{
		[DataMember(Name = "offset")]
		public int Offset { get; set; }
		
		[DataMember(Name = "count")]
		public int Count { get; set; }
		
		[DataMember(Name = "total_count", EmitDefaultValue = false, Order = 1)]
		public int? TotalCount { get; set; }
	}
}