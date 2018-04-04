using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Results.CodeReviewStatistics
{
	[DataContract]
	public class CodeReviewInstructorsStatisticsResult
	{
		[DataMember(Name = "instructors")]
		public List<CodeReviewInstructorStatistics> Instructors { get; set; }
		
		[DataMember(Name = "analyzed_code_reviews_count")]
		public int AnalyzedCodeReviewsCount { get; set; }
	}

	[DataContract]
	public class CodeReviewInstructorStatistics
	{
		[DataMember(Name = "instructor")]
		public ShortUserInfo Instructor { get; set; }
		
		[DataMember(Name = "exercises")]
		public List<CodeReviewExerciseStatistics> Exercises { get; set; }
	}

	[DataContract]
	public class CodeReviewExerciseStatistics
	{
		[DataMember(Name = "slide_id")]
		public Guid SlideId { get; set; }
		
		[DataMember(Name = "reviewed_submissions_count")]
		public int ReviewedSubmissionsCount { get; set; }
		
		[DataMember(Name = "queue_size")]
		public int QueueSize { get; set; }
		
		[DataMember(Name = "comments_count")]
		public int CommentsCount { get; set; }
	}
}