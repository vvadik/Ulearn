using System;
using System.ComponentModel.DataAnnotations;

namespace AntiPlagiarism.Web.Database.Models
{
	public class TaskStatisticsParameters
	{
		[Key]
		public Guid TaskId { get; set; }

		public double Mean { get; set; }

		public double Deviation { get; set; }
	}
}