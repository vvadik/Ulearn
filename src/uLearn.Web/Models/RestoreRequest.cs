using System;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class RestoreRequest
	{
		[Key]
		public string Id { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		[StringLength(64)]
		public string UserId { get; set; }

		[Required]
		public DateTime LastTry { get; set; }
	}
}