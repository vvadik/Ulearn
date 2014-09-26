using System;
using System.ComponentModel.DataAnnotations;

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

		[StringLength(64)]
		public string SlideId { get; set; }
		
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public DateTime Time { get; set; }
	}
}