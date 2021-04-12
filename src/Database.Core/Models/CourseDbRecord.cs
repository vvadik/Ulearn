using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class CourseDbRecord
	{
		[Key]
		[StringLength(100)]
		public string CourseId { get; set; }

		[Required]
		public string Name { get; set; }

		public bool IsArchived { get; set; }
	}
}