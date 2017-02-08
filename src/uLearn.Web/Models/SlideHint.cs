using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class SlideHint : ISlideAction
	{
		[Key]
		public int Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		[Index("FullIndex", 2)]
		public string UserId { get; set; }

		[Required]
		[Index("FullIndex", 3)]
		public int HintId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[Index("FullIndex", 1)]
		public Guid SlideId { get; set; }

		[Index("FullIndex", 4)]
		public bool IsHintHelped { get; set; }
	}
}