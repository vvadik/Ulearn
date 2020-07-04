using System;
using Microsoft.AspNetCore.Mvc;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class RecalculateTaskStatisticsParameters : AntiPlagiarismApiParameters
	{
		[FromQuery(Name = "from_task")]
		public Guid? FromTaskId { get; set; }

		[FromQuery(Name = "task_id")]
		public Guid? TaskId { get; set; }
	}
}