using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class EnabledAdditionalScoringGroup
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		
		[Required]
		[Index("IDX_EnabledAdditionalScoringGroup_ByGroup", 2)]
		public int GroupId { get; set; }

		public virtual Group Group { get; set; }

		[Required]
		public string ScoringGroupId { get; set; }
	}
}