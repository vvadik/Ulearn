using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database.Models;
using Database.Repos.Users;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Responses.Exercise
{
	[DataContract]
	public class SubmissionInfo
	{
		[DataMember]
		public int Id;

		[NotNull]
		[DataMember]
		public string Code;

		[DataMember]
		public DateTime Timestamp;

		[CanBeNull]
		[DataMember]
		public ExerciseAutomaticCheckingResponse AutomaticChecking; // null если задача не имеет автоматических тестов, это не отменяет возможности ревью.

		[DataMember]
		public bool ManualCheckingPassed;

		[NotNull]
		[DataMember]
		public List<ReviewInfo> ManualCheckingReviews;

		public static SubmissionInfo Build(UserExerciseSubmission submission,
			[CanBeNull] Dictionary<int, IEnumerable<ExerciseCodeReviewComment>> reviewId2Comments)
		{
			var reviews = submission
				.GetAllReviews()
				.Select(r =>
				{
					var comments = reviewId2Comments?.GetValueOrDefault(r.Id);
					var isUlearnBot = r.Author.UserName == UsersRepo.UlearnBotUsername;
					return ReviewInfo.Build(r, comments, isUlearnBot);
				})
				.ToList();
			var automaticChecking = submission.AutomaticChecking == null
				? null : ExerciseAutomaticCheckingResponse.Build(submission.AutomaticChecking, reviews.Where(r => r.Author == null).ToList());
			return new SubmissionInfo
			{
				Id = submission.Id,
				Code = submission.SolutionCode.Text,
				Timestamp = submission.Timestamp,
				AutomaticChecking = automaticChecking,
				ManualCheckingPassed = submission.ManualCheckings.Any(mc => mc.IsChecked),
				ManualCheckingReviews =  reviews.Where(r => r.Author != null).ToList()
			};
		}
	}
}