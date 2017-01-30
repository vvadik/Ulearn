using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class UnitAppearance
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		[Index("CourseAndTime", 1)]
		public string CourseId { get; set; }

		[Required]
		public Guid UnitId { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		[Index("CourseAndTime", 2)]
		public DateTime PublishTime { get; set; }
	}
}