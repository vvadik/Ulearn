using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.ExerciseStatistics
{
	[DataContract]
	public class CourseExercisesStatisticsResponse : SuccessResponse
	{
		[DataMember(Name = "exercises")]
		public List<OneExerciseStatistics> Exercises { get; set; }
		
		[DataMember(Name = "analyzed_submissions_count")]
		public int AnalyzedSubmissionsCount { get; set; }
	}

	[DataContract]
	public class OneExerciseStatistics
	{
		[DataMember(Name = "exercise")]
		public ShortSlideInfo Exercise { get; set; }

		[DataMember(Name = "submissions_count")]
		public int SubmissionsCount { get; set; }
		
		[DataMember(Name = "accepted_count")]
		public int AcceptedCount { get; set; }
		
		[DataMember(Name = "last_dates")]
		public Dictionary<DateTime, OneExerciseStatisticsForDate> LastDates { get; set; }
	}

	[DataContract]
	public class OneExerciseStatisticsForDate
	{
		[DataMember(Name = "submissions_count")]
		public int SubmissionsCount { get; set; }
		
		[DataMember(Name = "accepted_count")]
		public int AcceptedCount { get; set; }
	}
}