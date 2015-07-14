using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class Consumer
	{
		[Key]
		public int ConsumerId { get; set; }

		[Required]
		[StringLength(64)]
		public string Name { get; set; }

		[Required]
		[StringLength(64)]
		public string Key { get; set; }

		[Required]
		[StringLength(64)]
		public string Secret { get; set; }
	}
}