using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class AnalyticsTable
	{
		[Key]
		[Required]
		public string Id { get; set; }

		[Required]
		public IList<Visiter> Visiters { get; set; }

		[Required]
		public IList<SlideMark> Marks { get; set; }

		[Required]
		public IList<Solver> Solvers { get; set; } 
	}
}