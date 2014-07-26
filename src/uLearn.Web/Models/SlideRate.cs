using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public enum SlideRates
	{
		Good,
		NotUnderstand,
		Trivial,
		NotWatched
	}
	public class SlideRate
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public SlideRates Rate { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public int SlideId { get; set; }
	}
}