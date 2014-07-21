using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class Solver
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }
	}
}