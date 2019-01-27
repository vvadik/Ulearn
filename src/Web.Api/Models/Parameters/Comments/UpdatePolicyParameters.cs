using System.Runtime.Serialization;
using Database.Models;
using Database.Models.Comments;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Comments
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class UpdatePolicyParameters
	{
		[DataMember(Name = "are_comments_enabled")]
		public bool? AreCommentsEnabled { get; set; }
		
		[DataMember(Name = "moderation")]
		public CommentModerationPolicy? ModerationPolicy { get; set; }
		
		[DataMember(Name = "only_instructors_can_reply")]
		public bool? OnlyInstructorsCanReply { get; set; }
	}
}