using System;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class TempCourse
	{
		[Key]
		[StringLength(100)]
		public string CourseId { get; set; }

		[Required]
		public DateTime LoadingTime { get; set; } // Время загрузки новой версии
		
		public DateTime LastUpdateTime { get; set; } // Время загрузки курса с диска в web

		[Required]
		[StringLength(64)]
		public string AuthorId { get; set; }

		public virtual ApplicationUser Author { get; set; }

		public static string GetTmpCourseId(string baseCourseId, string userId)
		{
			return $"{baseCourseId}_{userId}";
		}
	}
}