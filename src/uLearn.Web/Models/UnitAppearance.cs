using System;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class UnitAppearance
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public string UnitName { get; set; }

		[Required]
		public string UserId { get; set; }

		public string UserName { get; set; }

		[Required]
		public DateTime PublishTime { get; set; }
	}
}