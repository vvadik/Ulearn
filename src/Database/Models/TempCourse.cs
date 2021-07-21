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
		public DateTime LoadingTime { get; set; } // Время загрузки новой версии в базу TempCoursesRepo.LoadingTime

		[Obsolete] // TODO удалить после того как будет писаться версия в meta
		public DateTime LastUpdateTime { get; set; } // Время загрузки курса с диска в web 

		[Required]
		[StringLength(64)]
		public string AuthorId { get; set; }

		public virtual ApplicationUser Author { get; set; }
	}
}