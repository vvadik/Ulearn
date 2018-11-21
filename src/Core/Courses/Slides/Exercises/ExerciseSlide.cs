using System;
using System.Linq;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Exercises
{
	[XmlRoot("slide", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class ExerciseSlide : Slide
	{
		[XmlAttribute("type")]
		public override SlideType Type { get; set; } = SlideType.Exercise;
		
		[XmlElement("scoring")]
		public ExerciseScoringSettings Scoring { get; set; } = new ExerciseScoringSettings
		{
			TestsScore = 5, 
		};
		
		protected override Type[] AllowedBlockTypes => base.AllowedBlockTypes.Concat(new[] { typeof(AbstractExerciseBlock) }).ToArray();
		
		public override bool ShouldBeSolved => true;

		public override string ScoringGroup => Scoring.ScoringGroup;

		public override int MaxScore => Scoring.TestsScore + Scoring.CodeReviewScore;

		[XmlIgnore]
		public AbstractExerciseBlock Exercise => (AbstractExerciseBlock) Blocks.Single(b => b is AbstractExerciseBlock);

		public ExerciseSlide()
		{
		}

		public ExerciseSlide(params SlideBlock[] blocks)
			: base(blocks)
		{
		}

		public override string ToString()
		{
			return $"ExerciseSlide: {Exercise}";
		}

		public override void BuildUp(CourseLoadingContext context)
		{
			if (string.IsNullOrEmpty(ScoringGroup))
				Scoring.ScoringGroup = context.CourseSettings.Scoring.DefaultScoringGroupForExercise;
			
			base.BuildUp(context);
		}

		public override void Validate(CourseLoadingContext context)
		{
			var scoringGroupsIds = context.CourseSettings.Scoring.Groups.Keys;
			var scoringGroup = Scoring.ScoringGroup;
			if (!string.IsNullOrEmpty(scoringGroup) && !scoringGroupsIds.Contains(scoringGroup))
				throw new CourseLoadingException(
					$"Неизвестная группа оценки у задания «{Title}»: {Scoring.ScoringGroup}\n" +
					"Возможные значения: " + string.Join(", ", scoringGroupsIds));

			var exerciseBlocksCount = Blocks.Count(b => b is AbstractExerciseBlock);
			if (exerciseBlocksCount == 0)
			{
				throw new CourseLoadingException(
					$"Не найдено блоков с упражнениями (<single-file-exercise> или <proj-exercise>) в слайде «{Title}», " +
					"для которого указан тип \"exercise\" (<slide type=\"exercise\">).");
			}
			if (exerciseBlocksCount > 1)
			{
				throw new CourseLoadingException(
					"Блок с упражнением (<single-file-exercise> или <proj-exercise>) может быть только один на слайде. " +
					$"Но на слайде {Title} найдено {exerciseBlocksCount} таких блока.");
			}
			
			base.Validate(context);
		}
		
		public Component GetSolutionsComponent(string displayName, Slide slide, int componentIndex, string launchUrl,
			string ltiId)
		{
			return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex + "-solutions", launchUrl, ltiId, false, 0, false);
		}

		public Component GetExerciseComponent(string displayName, Slide slide, int componentIndex, string launchUrl,
			string ltiId)
		{
			return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex, launchUrl, ltiId, true, Scoring.TestsScore, false);
		}
	}
}