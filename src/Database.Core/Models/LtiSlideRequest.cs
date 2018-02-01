using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class LtiSlideRequest
	{
		[Key]
		public int RequestId { get; set; }

		[Required]
		public Guid SlideId { get; set; }

		[Required]
		[StringLength(64)]
		public string UserId { get; set; }

		[Required]
		public string Request { get; set; }
	}
}