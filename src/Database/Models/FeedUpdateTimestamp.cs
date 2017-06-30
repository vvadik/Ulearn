using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class FeedViewTimestamp
	{
		[Key]
		[StringLength(64)]
		[Index("IDX_FeedUpdateTimestamp_ByUser")]
		public string UserId { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
