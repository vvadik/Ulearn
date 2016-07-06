using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class Group
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_Group_GroupByCourse")]
		public string CourseId { get; set; }

		[Required]
		[StringLength(300)]
		public string Name { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_Group_GroupByOwner")]
		public string OwnerId { get; set; }

		public virtual ApplicationUser Owner { get; set; }

		[Required]
		public bool IsPublic { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Index("IDX_Group_GroupByInviteHash")]
		public Guid InviteHash { get; set; }

		public virtual ICollection<GroupMember> Members { get; set; }
	}
}