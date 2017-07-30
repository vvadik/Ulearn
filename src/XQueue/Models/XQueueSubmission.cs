using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XQueue.Models
{
	[DataContract]
	public class XQueueSubmission
	{
		[DataMember(Name = "xqueue_header")]
		public XQueueHeader Header { get; set; }

		[DataMember(Name = "xqueue_files")]
		public Dictionary<string, string> Files { get; set; }

		[DataMember(Name = "xqueue_body")]
		public XQueueSubmissionBody Body { get; set; }
	}

	[DataContract]
	public class XQueueHeader
	{
		[DataMember(Name = "submission_id")]
		public string SubmissionId { get; set; }

		[DataMember(Name = "submission_key")]
		public string SubmissionKey { get; set; }
	}

	[DataContract]
	public class XQueueSubmissionBody
	{
		[DataMember(Name = "student_info")]
		public XQueueStudentInfo StudentInfo { get; set; }

		[DataMember(Name = "student_response")]
		public string StudentResponse { get; set; }

		[DataMember(Name = "grader_payload")]
		public string GraderPayload { get; set; }
	}

	[DataContract]
	public class XQueueStudentInfo
	{
		[DataMember(Name = "anonymous_student_id")]
		public string AnonymousStudentId { get; set; }

		[DataMember(Name = "submission_time")]
		public string SubmissionTime { get; set; }

		[DataMember(Name = "random_seed")]
		public string RandomSeed { get; set; }
	}
}