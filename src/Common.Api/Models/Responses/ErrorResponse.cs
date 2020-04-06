using System;
using System.Runtime.Serialization;
using Vostok.Context;
using Vostok.Tracing.Abstractions;

//using Vostok.Tracing;

namespace Ulearn.Common.Api.Models.Responses
{
	[DataContract]
	public class ErrorResponse : ApiResponse
	{
		[DataMember(Name = "status")]
		public ResponseStatus Status { get; } = ResponseStatus.Error;

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "trace_id")]
		public Guid TraceId => FlowingContext.Globals.Get<TraceContext>().TraceId;

		[DataMember(Name = "timestamp")]
		public DateTime Timestamp { get; } = DateTime.Now;

		public ErrorResponse(string message)
		{
			Message = message;
		}
	}
}