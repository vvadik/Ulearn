using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class Consumer
	{
		[Key]
		public int ConsumerId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Key { get; set; }

		[Required]
		public string Secret { get; set; }
	}
}