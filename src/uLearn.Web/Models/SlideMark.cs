using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public enum SlideMarks
	{
		Good,
		NotUnderstand,
		Trivial,
		NotWatched
	}
	public class SlideMark
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public SlideMarks Mark { get; set; }

		[Required]
		public string UserId { get; set; }
	}
}