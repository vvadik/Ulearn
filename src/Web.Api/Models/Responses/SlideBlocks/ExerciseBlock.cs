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
		public Language? Language { get; set; }

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

		[DataMember]
		public bool WaitingForManualChecking { get; set; }

		[CanBeNull]
		[DataMember]
		public string CorrectSolution { get; set; } // Устанавливается, если перподаватель и у задачи есть авторское решение

		public ExerciseBlockResponse(AbstractExerciseBlock exerciseBlock, ExerciseSlideRendererContext context)
		{
			var reviewId2Comments = context.CodeReviewComments
				?.GroupBy(c => c.ReviewId)
				.ToDictionary(g => g.Key, g => g.AsEnumerable());

			Hints = exerciseBlock.Hints.ToArray();
			ExerciseInitialCode = exerciseBlock.ExerciseInitialCode;
			HideSolutions = exerciseBlock.HideShowSolutionsButton;
			ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : exerciseBlock.ExpectedOutput?.NormalizeEoln();
			Language = exerciseBlock.Language;
			AttemptsStatistics = context.AttemptsStatistics;
			WaitingForManualChecking = context.Submissions?.FirstOrDefault()?.ManualCheckings.Any(c => !c.IsChecked) ?? false;
			CorrectSolution = GetSolution(context.IsInstructor, exerciseBlock);
			Submissions = context.Submissions
				.EmptyIfNull()
				.Select(s => SubmissionInfo.Build(s, reviewId2Comments))
				.ToList();
		}

		[CanBeNull]
		private static string GetSolution(bool isInstructor, AbstractExerciseBlock exerciseBlock)
		{
			if (!isInstructor)
				return null;
			return exerciseBlock switch
			{
				UniversalExerciseBlock u => u.GetCorrectSolution(),
				CsProjectExerciseBlock c => c.CorrectSolutionFile.Exists ? c.CorrectSolutionFile.ContentAsUtf8() : null,
				_ => null
			};
		}
	}
}