using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupInfo
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }

		[DataMember(Name = "create_time")]
		public DateTime? CreateTime { get; set; }
		
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "is_archived")]
		public bool IsArchived { get; set; }

		[DataMember(Name = "owner")]
		public ShortUserInfo Owner { get; set; }
		
		[DataMember(Name = "invite_hash")]
		public Guid InviteHash { get; set; }
		
		[DataMember(Name = "is_invite_link_enabled")]
		public bool IsInviteLinkEnabled { get; set; }
		
	
		[DataMember(Name = "is_manual_checking_enabled")]
		public bool IsManualCheckingEnabled { get; set; }
		
		[DataMember(Name = "is_manual_checking_enabled_for_old_solutions")]
		public bool IsManualCheckingEnabledForOldSolutions { get; set; }
		
		[DataMember(Name = "default_prohibit_further_review")]
		public bool DefaultProhibitFurtherReview { get; set; }
		
		[DataMember(Name = "can_students_see_group_progress")]
		public bool CanStudentsSeeGroupProgress { get; set; }
		
		
		[DataMember(Name = "students_count", EmitDefaultValue = false)]
		public int? StudentsCount { get; set; }
		
		[DataMember(Name = "accesses", EmitDefaultValue = false)]
		public List<GroupAccessesInfo> Accesses { get; set; }
		
		
		[DataMember(Name = "api_url", EmitDefaultValue = false)]
		public string ApiUrl { get; set; }
	}
}