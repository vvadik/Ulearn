using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class SlideHint
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		public int HintId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[StringLength(64)]
		public string SlideId { get; set; }

		public bool IsHintHelped { get; set; }
	}
}