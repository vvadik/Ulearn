using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class RecalculateTaskStatisticsResult : ApiSuccessResult
	{
		[DataMember(Name = "task_ids")]
		public List<Guid> TaskIds { get; set; }
	}
}