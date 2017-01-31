using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using RunCsJob.Api;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
	public abstract class ExerciseBlock : IncludeCode
	{
		protected ExerciseBlock()
		{
			MaxScore = 5;
			MaxReviewAttempts = 2;
			CorrectnessScore = MaxScore;
		}

		[XmlElement("max-score")]
		public int MaxScore { get; set; }

		[XmlElement("inital-code")]
		public string ExerciseInitialCode { get; set; }

		[XmlElement("hint")]
		public List<string> Hints { get; set; }

		[XmlElement("comment")]
		public string CommentAfterExerciseIsSolved { get; set; }

		[XmlElement("expected")]
		// Ожидаемый корректный вывод программы
		public string ExpectedOutput { get; set; }

		[XmlElement("hide-expected-output")]
		public bool HideExpectedOutputOnError { get; set; }

		[XmlElement("validator")]
		public string ValidatorName { get; set; }

		[XmlElement("hide-solutions")]
		public bool HideShowSolutionsButton { get; set; }

		[XmlElement("require-review")]
		public bool RequireReview { get; set; }

		[XmlElement("correctness-score")]
		public int CorrectnessScore { get; set; }

		[XmlElement("max-review-attempts")]
		public int MaxReviewAttempts { get; set; }

		[XmlElement("scoring-group")]
		public string ScoringGroup { get; set; }

		public int MaxReviewScore => MaxScore - CorrectnessScore;

		public List<string> HintsMd
		{
			get { return Hints = Hints ?? new List<string>(); }
			set { Hints = value; }
		}

		public abstract string GetSourceCode(string code);

		public abstract SolutionBuildResult BuildSolution(string userWrittenCode);

		public abstract RunnerSubmission CreateSubmition(string submissionId, string code);

		#region equals

		private bool Equals(ExerciseBlock other)
		{
			return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsMd, other.HintsMd);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			return obj.GetType() == GetType() && Equals((ExerciseBlock)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ExerciseInitialCode?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (ExpectedOutput?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (HintsMd?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		#endregion

		public override string ToString()
		{
			return $"Exercise: {ExerciseInitialCode}, Hints: {string.Join("; ", HintsMd)}";
		}

		public Component GetSolutionsComponent(string displayName, Slide slide, int componentIndex, string launchUrl, string ltiId)
		{
			return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex + 1, launchUrl, ltiId, false, 0, false);
		}

		public Component GetExerciseComponent(string displayName, Slide slide, int componentIndex, string launchUrl, string ltiId)
		{
			return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex, launchUrl, ltiId, true, CorrectnessScore, false);
		}

		public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
		{
			throw new NotSupportedException();
		}

		public override string TryGetText()
		{
			return (ExerciseInitialCode ?? "") + '\n'
					+ string.Join("\n", HintsMd) + '\n'
					+ (CommentAfterExerciseIsSolved ?? "");
		}

		protected void CheckScoringGroup(BuildUpContext context)
		{
			var scoringGroups = context.CourseSettings.Scoring.Groups.Select(g => g.Id).ToList();
			if (!string.IsNullOrEmpty(ScoringGroup) && ! scoringGroups.Contains(ScoringGroup))
				throw new CourseLoadingException($"Неизвестная группа оценки у задания {context.Lesson.Title}: {ScoringGroup}\n" +
												 "Возможные значения: " + string.Join(", ", scoringGroups));

			if (string.IsNullOrEmpty(ScoringGroup))
				ScoringGroup = context.CourseSettings.Scoring.DefaultScoringGroupForExercise;
		}
	}
}