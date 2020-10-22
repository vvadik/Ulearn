using System;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Web.Api.Controllers;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Review
{
	[DataContract]
	public class ReviewCommentResponse
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int ReviewId { get; set; }

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public DateTime PublishTime { get; set; }

		[DataMember]
		public ShortUserInfo Author { get; set; }

		public static ReviewCommentResponse Build(ExerciseCodeReviewComment comment)
		{
			return new ReviewCommentResponse
			{
				Id = comment.Id,
				ReviewId = comment.ReviewId,
				Text = comment.Text,
				PublishTime = comment.AddingTime,
				Author = BaseController.BuildShortUserInfo(comment.Author)
			};
		}
	}
}