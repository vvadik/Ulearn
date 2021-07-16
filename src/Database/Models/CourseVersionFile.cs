using System;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class CourseVersionFile
	{
		[Key]
		[Required]
		public Guid CourseVersionId { get; set; }

		public virtual CourseVersion CourseVersion { get; set; }

		[Required]
		[StringLength(100)]
		public string CourseId { get; set; }

		[Required]
		public byte[] File { get; set; }
	}
}