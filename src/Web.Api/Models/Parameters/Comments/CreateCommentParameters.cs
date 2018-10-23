using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Parameters.Comments
{
	[DataContract]
	public class CreateCommentParameters
	{
		[DataMember(Name = "text", IsRequired = true)]
		public string Text { get; set; }

		[DataMember(Name = "reply_to")]
		public int? ParentCommentId { get; set; }

		[DataMember(Name = "for_instructors")]
		public bool IsForInstructorsOnly { get; set; }
	}
}