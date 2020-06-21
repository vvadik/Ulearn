using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiPlagiarism.Web.Database.Models
{
	public class ManualSuspicionLevels
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid TaskId { get; set; }

		public double? FaintSuspicion { get; set; }

		public double? StrongSuspicion { get; set; }

		public DateTime Timestamp { get; set; }
	}
}