using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class UserQuestion
	{
		[Key]
		[Required]
		public int QuestionId { get; set; }

		[Required]
		public string SlideTitle { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		public string UserId { get; set; }

		[StringLength(64)]
		[Required]
		public string UserName { get; set; }

		[Required]
		public string Question { get; set; }

		[Required]
		public string UnitName { get; set; }

		[Required]
		public DateTime Time { get; set; }
	}
}