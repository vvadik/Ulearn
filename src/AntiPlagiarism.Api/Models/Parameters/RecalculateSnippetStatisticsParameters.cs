using System;
using Microsoft.AspNetCore.Mvc;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class RecalculateSnippetStatisticsParameters : ApiParameters
	{
		[FromQuery(Name = "from_task")]
		public Guid? FromTaskId { get; set; }
	}
}