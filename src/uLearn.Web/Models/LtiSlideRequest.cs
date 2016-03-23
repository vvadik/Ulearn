using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class LtiSlideRequest
	{
		private const string SlideUserIndexName = "IDX_LtiSlideRequest_SlideAndUser";

		[Key]
		public int RequestId { get; set; }

		[Required]
		[Index(SlideUserIndexName, 1)]
		public Guid SlideId { get; set; }

		[Required]
		[StringLength(64)]
		[Index(SlideUserIndexName, 2)]
		public string UserId { get; set; }

		[Required]
		public string Request { get; set; }
	}
}
