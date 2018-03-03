using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class UserRole
	{
		[Required]
		[Key]
		public int Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		public string UserId { get; set; }

		[Required]
		public string CourseId { get; set; }

		[Required]
		public CourseRole Role { get; set; }
	}
}