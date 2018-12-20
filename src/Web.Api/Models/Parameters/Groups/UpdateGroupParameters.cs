using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Validations;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class UpdateGroupParameters
	{
		[DataMember(Name = "name")]
		[NotEmpty(ErrorMessage = "Group name can not be empty", CanBeNull = true)]
		public string Name { get; set; }
		
		[DataMember(Name = "is_archived")]
		public bool? IsArchived { get; set; }
		
		[DataMember(Name = "is_invite_link_enabled")]
		public bool? IsInviteLinkEnabled { get; set; }
		
		
		[DataMember(Name = "is_manual_checking_enabled")]
		public bool? IsManualCheckingEnabled { get; set; }
		
		[DataMember(Name = "is_manual_checking_enabled_for_old_solutions")]
		public bool? IsManualCheckingEnabledForOldSolutions { get; set; }
		
		[DataMember(Name = "default_prohibit_further_review")]
		public bool? DefaultProhibitFurtherReview { get; set; }
		
		[DataMember(Name = "can_students_see_group_progress")]
		public bool? CanStudentsSeeGroupProgress { get; set; }
	}
}