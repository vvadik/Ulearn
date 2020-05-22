using System;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class TempCourse
	{
		[Key]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public DateTime LoadingTime { get; set; }
		
		public DateTime LastUpdateTime { get; set; }

		[Required]
		[StringLength(64)]
		public string AuthorId { get; set; }

		public virtual ApplicationUser Author { get; set; }
	}
}