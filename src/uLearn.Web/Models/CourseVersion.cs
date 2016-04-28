using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class CourseVersion
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_CourseVersion_ByCourseAndPublishTime", 1)]
		[Index("IDX_CourseVersion_ByCourseAndLoadingTime", 1)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_CourseVersion_ByCourseAndLoadingTime", 2)]
		public DateTime LoadingTime { get; set; }

		[Index("IDX_CourseVersion_ByCourseAndPublishTime", 2)]
		public DateTime? PublishTime { get; set; }

		[Required]
		[StringLength(64)]
		public string AuthorId { get; set; }

		public virtual ApplicationUser Author { get; set; }
	}
}