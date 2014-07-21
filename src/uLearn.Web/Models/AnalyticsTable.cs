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


		public virtual IList<Visiter> Visiters { get; set; }


		public virtual IList<SlideMark> Marks { get; set; }


		public virtual IList<Solver> Solvers { get; set; }
	}
}