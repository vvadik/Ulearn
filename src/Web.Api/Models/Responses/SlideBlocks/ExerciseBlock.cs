using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Web.Api.Controllers.Slides;
using Ulearn.Web.Api.Models.Responses.Exercise;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	[DisplayName("exercise")]
	public class ExerciseBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[DataMember]
		public Language[] Languages { get; set; }

		[CanBeNull]
		[DataMember]
		public Dictionary<Language, string> LanguageNames { get; set; } // Для языка содержит его текстовое название, если оно не такое же, как поле enum

		[NotNull]
		[DataMember]
		public string[] Hints { get; set; }

		[NotNull]
		[DataMember]
		public string ExerciseInitialCode { get; set; }

		[DataMember]
		public bool HideSolutions { get; set; }

		[CanBeNull]
		[DataMember]
		public string ExpectedOutput { get; set; } // В том числе может быть не пуст, но скрыт от студента, тогда и здесь null

		[NotNull]
		[DataMember]
		public List<SubmissionInfo> Submissions { get; set; }

		[NotNull]
		[DataMember]
		public ExerciseAttemptsStatistics AttemptsStatistics { get; set; }

		public ExerciseBlockResponse(AbstractExerciseBlock exerciseBlock,
			ExerciseSlideRendererContext context)
		{
			var reviewId2Comments = context.CodeReviewComments
				?.GroupBy(c => c.ReviewId)
				.ToDictionary(g => g.Key, g => g.AsEnumerable());

			Hints = exerciseBlock.Hints.ToArray();
			ExerciseInitialCode = exerciseBlock.ExerciseInitialCode.RemoveEmptyLinesFromStart().TrimEnd().EnsureEnoughLines(4);
			HideSolutions = exerciseBlock.HideShowSolutionsButton;
			ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseBlock.ExpectedOutput?.NormalizeEoln();
			Languages = exerciseBlock.Language != null ? new[] { exerciseBlock.Language.Value } : new Language[0];
			LanguageNames = null;
			AttemptsStatistics = context.AttemptsStatistics;
			Submissions = context.Submissions
				.EmptyIfNull()
				.Select(s => SubmissionInfo.Build(s, reviewId2Comments))
				.ToList();
		}
	}
}