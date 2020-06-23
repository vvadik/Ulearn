using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class SetSuspicionLevelsParameters : AntiPlagiarismApiParameters
	{
		[BindRequired]
		[FromQuery(Name = "task_id")]
		public Guid TaskId { get; set; }

		[FromQuery(Name = "faint_suspicion")]
		public double? FaintSuspicion { get; set; }

		[FromQuery(Name = "strong_suspicion")]
		public double? StrongSuspicion { get; set; }
	}
}