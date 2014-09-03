using System;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class Like
	{
		[Key]
		public int Id { get; set; }

		public virtual UserSolution UserSolution { get; set; }
		
		[Required]
		public int UserSolutionId { get; set; }

		[Required]
		[StringLength(64)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}