using Microsoft.AspNetCore.Mvc;
using Ulearn.Web.Api.Models.Validations;

namespace Ulearn.Web.Api.Models.Parameters.Comments
{
	public class SlideCommentsParameters : IPaginationParameters
	{
		[FromQuery(Name = "for_instuctors")]
		public bool ForInstructors { get; set; }
		
		[FromQuery(Name = "offset")]
		[MinValue(0, ErrorMessage = "Offset should be non-negative")]
		public int Offset { get; set; }

		[FromQuery(Name = "count")]
		[MinValue(0, ErrorMessage = "Count should be non-negative")]
		[MaxValue(200, ErrorMessage = "Count should be at most 200")]
		public int Count { get; set; } = 100;
	}
}