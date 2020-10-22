using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database.Models;
using Database.Repos;
using JetBrains.Annotations;
using Ulearn.Core.Courses.Slides.Exercises;

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

		[NotNull]
		[DataMember]
		public List<ReviewInfo> Reviews;

		[CanBeNull]
		[DataMember]
		public ExerciseAutomaticCheckingResponse AutomaticChecking; // null если задача не имеет автоматических тестов, это не отменяет возможности ревью.

		[DataMember]
		public bool ManualCheckingPassed;

		[DataMember]
		public int? ManualCheckingPercent; // Процент от максимального балла, поставленный преподавателем.

		public static SubmissionInfo Build(UserExerciseSubmission submission,
			ExerciseSlide slide,
			[CanBeNull] Dictionary<int, IEnumerable<ExerciseCodeReviewComment>> reviewId2Comments)
		{
			var reviews = submission
				.GetAllReviews()
				.Select(r =>
				{
					var comments = reviewId2Comments?.GetValueOrDefault(r.Id);
					return ReviewInfo.Build(r, comments);
				})
				.ToList();
			var automaticChecking = submission.AutomaticChecking == null ? null : ExerciseAutomaticCheckingResponse.Build(submission.AutomaticChecking);
			return new SubmissionInfo
			{
				Id = submission.Id,
				Code = submission.SolutionCode.Text,
				Timestamp = submission.Timestamp,
				Reviews = reviews,
				AutomaticChecking = automaticChecking,
				ManualCheckingPassed = submission.ManualCheckings.Any(mc => mc.IsChecked),
				ManualCheckingPercent = SlideCheckingsRepo.GetExerciseSubmissionManualCheckingsScoreAndPercent(submission.ManualCheckings, slide).Percent
			};
		}
	}
}