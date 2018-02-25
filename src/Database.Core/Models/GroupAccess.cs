using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class GroupAccess
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public int GroupId { get; set; }

		public virtual Group Group { get; set; }

		[StringLength(64)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		public string GrantedById { get; set; }

		public virtual ApplicationUser GrantedBy { get; set; }

		[Required]
		public GroupAccessType AccessType { get; set; }

		public DateTime GrantTime { get; set; }

		[Required]
		public bool IsEnabled { get; set; }
	}

	public enum GroupAccessType : short
	{
		FullAccess = 1,

		/* Can't be stored in database. Only for internal needs */
		Owner = 100,
	}
}
