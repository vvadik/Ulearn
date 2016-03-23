using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class Visit : ISlideAction
	{
		[Key]
		public int Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		[Index("IDX_Visits_UserAndSlide", 1)]
		public string UserId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_Visits_UserAndSlide", 2)]
		[Index("IDX_Visits_SlideAndTime", 1)]
		public Guid SlideId { get; set; }

		///<summary>Первый заход на слайд</summary>
		[Required]
		[Index("IDX_Visits_SlideAndTime", 2)]
		public DateTime Timestamp { get; set; }

		public int Score { get; set; }
		public int AttemptsCount { get; set; }
		public bool IsSkipped { get; set; }
		public bool IsPassed { get; set; }
	}
}