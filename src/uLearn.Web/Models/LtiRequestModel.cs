using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class LtiRequestModel
	{
		[Key]
		public int RequestId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("SlideAndUser", 2)]
		public string UserId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("SlideAndUser", 1)]
		public string SlideId { get; set; }

		[Required]
		public string Request { get; set; }
	}
}
