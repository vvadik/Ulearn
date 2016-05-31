using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class ManualQuizCheckQueueItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_ManualQuizCheck_ManualQuizCheckBySlide")]
		[Index("IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime", 1)]
		[Index("IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser", 1)]
		public Guid SlideId { get; set; }

		[Required]
		[Index("IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime", 2)]
		public DateTime Timestamp { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser", 2)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		public DateTime? LockedUntil { get; set; }
		
		[StringLength(64)]
		public string LockedById { get; set; }

		public virtual ApplicationUser LockedBy { get; set; }

		public bool IsChecked { get; set; }

		public int Score { get; set; }
	}
}