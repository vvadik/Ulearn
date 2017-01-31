﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class AdditionalScore
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_AdditionalScore_ByCourseAndUser", 1)]
		[Index("IDX_AdditionalScore_ByCourseUnitAndUser", 1, IsUnique = true)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_AdditionalScore_ByUnit")]
		[Index("IDX_AdditionalScore_ByUnitAndUser", 1)]
		[Index("IDX_AdditionalScore_ByCourseUnitAndUser", 2, IsUnique = true)]
		public Guid UnitId { get; set; }

		public string ScoringGroupId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_AdditionalScore_ByUnitAndUser", 2)]
		[Index("IDX_AdditionalScore_ByCourseAndUser", 2)]
		[Index("IDX_AdditionalScore_ByCourseUnitAndUser", 3, IsUnique = true)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		public int Score { get; set; }

		[Required]
		[StringLength(64)]
		public string InstructorId { get; set; }

		public virtual ApplicationUser Instructor { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}