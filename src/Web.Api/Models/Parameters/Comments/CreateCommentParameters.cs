using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Common.Api.Models.Validations;

namespace Ulearn.Web.Api.Models.Parameters.Comments
{
	[DataContract]
	public class CreateCommentParameters
	{
		[DataMember(Name = "text", IsRequired = true)]
		[NotEmpty(ErrorMessage = "Text can not be empty")]
		[MaxLength(CommentsPolicy.MaxCommentLength, ErrorMessage = "Comment is too large. Max allowed length is 10000 chars")]
		public string Text { get; set; }

		[DataMember(Name = "reply_to")]
		public int? ParentCommentId { get; set; }

		[DataMember(Name = "for_instructors")]
		public bool IsForInstructorsOnly { get; set; }
	}
}