using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class Like
	{
		[Key]
		[Required]
		public int SolutionId { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}