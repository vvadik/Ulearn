using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AntiPlagiarism.Api.Models.Parameters
{
	public class GetAuthorPlagiarismsParameters : ApiParameters
	{
		[BindRequired]
		[FromQuery(Name = "author_id")]
		public Guid AuthorId { get; set; }
		
		[BindRequired]
		[FromQuery(Name = "task_id")]
		public Guid TaskId { get; set; }
	}
}
