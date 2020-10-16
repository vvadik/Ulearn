using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database.Models;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	[DataContract]
	public class SubmissionInfo
	{
		[DataMember]
		public int Id;

		[DataMember]
		public string Code;

		[DataMember]
		public DateTime Timestamp;

		[DataMember]
		public List<ReviewInfo> Reviews;

		[DataMember]
		public string Output;

		[DataMember]
		public float? Points;

		public static SubmissionInfo BuildSubmissionInfo(UserExerciseSubmission submission,
			[CanBeNull] Dictionary<int, IEnumerable<ExerciseCodeReviewComment>> reviewId2Comments)
		{
			var reviews = submission
				.GetAllReviews()
				.Select(r =>
				{
					var comments = reviewId2Comments?.GetValueOrDefault(r.Id);
					return ReviewInfo.BuildReviewInfo(r, comments);
				})
				.ToList();
			return new SubmissionInfo
			{
				Id = submission.Id,
				Code = submission.SolutionCode.Text,
				Timestamp = submission.Timestamp,
				Reviews = reviews,
				Output = submission.AutomaticChecking.Output?.Text,
				Points = submission.AutomaticChecking.Points,
			};
		}
	}
}