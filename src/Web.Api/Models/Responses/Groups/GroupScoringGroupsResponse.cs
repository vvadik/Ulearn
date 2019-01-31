using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;

namespace Ulearn.Web.Api.Models.Responses.Groups
{
	[DataContract]
	public class GroupScoringGroupsResponse : SuccessResponse
	{
		[DataMember(Name = "scores")]
		public List<GroupScoringGroupInfo> ScoringGroups { get; set; }
	}

	[DataContract]
	public abstract class AbstractScoringGroupInfo
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }
		
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "abbreviation")]
		public string Abbreviation { get; set; }
		
		[DataMember(Name = "description")]
		public string Description { get; set; }
	}
	
	[DataContract]
	public class GroupScoringGroupInfo : AbstractScoringGroupInfo
	{
		[DataMember(Name = "are_additional_scores_enabled_for_all_groups")]
		public bool IsEnabledForEveryone { get; set; }
		
		[DataMember(Name = "can_instructor_set_additional_score_in_some_unit")]
		public bool CanBeSetByInstructorInSomeUnit { get; set; }
		
		[DataMember(Name = "are_additional_scores_enabled_in_this_group", EmitDefaultValue = false)]
		public bool? IsEnabled { get; set; }
	}

	[DataContract]
	public class UnitScoringGroupInfo : AbstractScoringGroupInfo
	{
		[DataMember(Name = "can_instructor_set_additional_score")]
		public bool CanBeSetByInstructor { get; set; }
		
		[DataMember(Name = "max_additional_score")]
		public int MaxAdditionalScore { get; set; }
	}
}