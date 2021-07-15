using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Submissions
{
	[DataContract]
	public class FavouriteReviewsResponse
	{
		public List<FavouriteReview> FavouriteReviews;
	}

	[DataContract]
	public class FavouriteReview
	{
		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public string RenderedText { get; set; }

		[DataMember]
		public bool IsFavourite { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int UseCount { get; set; }
	}
}