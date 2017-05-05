using System.Runtime.Serialization;

namespace XQueue.Models
{
	[DataContract]
	public class XQueueResult
	{
		[DataMember(Name = "xqueue_header")]
		public XQueueHeader Header { get; set; }

		[DataMember(Name = "xqueue_body")]
		public XQueueResultBody Body { get; set; }
	}

	[DataContract]
	public class XQueueResultBody
	{
		[DataMember(Name = "correct")]
		public bool IsCorrect { get; set; }

		[DataMember(Name = "score")]
		public float Score { get; set; }

		[DataMember(Name = "msg")]
		public string Message { get; set; }
	}
}
