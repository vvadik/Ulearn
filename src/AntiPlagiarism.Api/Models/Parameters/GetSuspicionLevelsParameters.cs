using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Ulearn.Common;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class GetSuspicionLevelsParameters : AntiPlagiarismApiParameters
	{
		[BindRequired]
		[FromQuery(Name = "task_id")]
		public Guid TaskId { get; set; }

		[BindRequired]
		[FromQuery(Name = "language")]
		public Language Language { get; set; }
	}
}