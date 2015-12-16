using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class Visiters : ISlideAction
	{
		[Key]
		public int Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		[Index("SlideAndUser", 1)]
		public string UserId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("SlideAndUser", 2)]
		[Index("SlideAndTime", 1)]
		public string SlideId { get; set; }

		[Required]
		[Index("SlideAndTime", 2)]
		public DateTime Timestamp { get; set; }

		public int Score { get; set; }
		public int AttemptsCount { get; set; }
		public bool IsSkipped { get; set; }
		public bool IsPassed { get; set; }
	}
}