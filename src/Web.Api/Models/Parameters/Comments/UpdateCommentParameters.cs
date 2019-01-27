using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Database.Models;
using Database.Models.Comments;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Validations;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Comments
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class UpdateCommentParameters
	{
		[CanBeNull]
		[DataMember(Name = "text")]
		[NotEmpty(CanBeNull = true, ErrorMessage = "Text can not be empty")]
		[MaxLength(CommentsPolicy.MaxCommentLength, ErrorMessage = "Comment is too large. Max allowed length is 10000 chars")]
		public string Text { get; set; }
		
		[DataMember(Name = "is_approved")]
		public bool? IsApproved { get; set; }
		
		[DataMember(Name = "is_pinned_to_top")]
		public bool? IsPinned { get; set; }
		
		[DataMember(Name = "is_correct_answer")]
		public bool? IsCorrectAnswer { get; set; }
	}
}