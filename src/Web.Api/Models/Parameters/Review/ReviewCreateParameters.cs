using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Database.Models.Comments;
using Ulearn.Common.Api.Models.Validations;

namespace Ulearn.Web.Api.Models.Parameters.Review
{
	[DataContract]
	public class ReviewCreateParameters
	{
		[DataMember(IsRequired = true)]
		[NotEmpty(ErrorMessage = "Text can not be empty")]
		[MaxLength(CommentsPolicy.MaxCommentLength, ErrorMessage = "Comment is too large. Max allowed length is 10000 chars")]
		public string Text { get; set; }
		
		public int StartLine { get; set; }
		public int StartPosition { get; set; }
		public int FinishLine { get; set; }
		public int FinishPosition { get; set; }
	}
}