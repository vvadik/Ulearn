using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class Visiters
	{
		[Key]
		public int Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		public string UserId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[StringLength(64)]
		public string SlideId { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}