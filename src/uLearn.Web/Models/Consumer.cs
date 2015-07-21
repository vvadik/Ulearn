using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class Consumer
	{
		[Key]
		public int ConsumerId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("FullIndex", 1)]
		public string Name { get; set; }

		[Required]
		[StringLength(64)]
		[Index("FullIndex", 2)]
		public string Key { get; set; }

		[Required]
		[StringLength(64)]
		[Index("FullIndex", 3)]
		public string Secret { get; set; }
	}
}