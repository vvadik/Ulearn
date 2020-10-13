using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Database.Models;
using JetBrains.Annotations;
using Ulearn.Common;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Web.Api.Controllers.Slides;
using Ulearn.Web.Api.Models.Responses.Exercise;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class ExerciseBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "exercise";

		[DataMember(Name = "language")]
		public Language? Language { get; set; }

		[DataMember(Name = "hints")]
		public string[] Hints { get; set; }

		[DataMember(Name = "exerciseInitialCode")]
		public string ExerciseInitialCode { get; set; }

		[DataMember(Name = "submissions")]
		public List<SubmissionInfo> Submissions { get; set; }

		[DataMember(Name = "attemptsStatistics")]
		public ExerciseAttemptsStatistics AttemptsStatistics { get; set; }

		public ExerciseBlockResponse(AbstractExerciseBlock exerciseBlock,
			ExerciseSlideRendererContext context)
		{
			var reviewId2Comments = context.CodeReviewComments
				?.GroupBy(c => c.ReviewId)
				.ToDictionary(g => g.Key, g => g.AsEnumerable());

			Hints = exerciseBlock.Hints.ToArray();
			ExerciseInitialCode = exerciseBlock.ExerciseInitialCode;
			Language = exerciseBlock.Language;
			AttemptsStatistics = context.AttemptsStatistics;
			Submissions = context.Submissions
				.EmptyIfNull()
				.Select(s => SubmissionInfo.BuildSubmissionInfo(s, reviewId2Comments))
				.ToList();
		}
	}
}