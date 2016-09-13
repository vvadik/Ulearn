using System.Collections.Generic;
using System.Web.Mvc;

namespace uLearn.Web.Models
{
	public class ExerciseBlockData
	{
		public ExerciseBlockData(string courseId, int slideIndex, bool showControls = true, bool isSkipped = true, string solution = null)
		{
			CourseId = courseId;
			SlideIndex = slideIndex;
			ShowControls = showControls;
			Solution = solution;
			CanSkip = !isSkipped && Solution == null;
			ReviewState = ExerciseReviewState.NotReviewed;
		}

		public bool ShowControls { get; private set; }
		public bool CanSkip { get; private set; }
		public string Solution { get; private set; }
		public UrlHelper Url { get; set; }
		public HtmlHelper Help { get; set; }
		private string runSolutionUrl;
		public string RunSolutionUrl {
			get
			{
				if (! string.IsNullOrEmpty(runSolutionUrl) || Url == null)
					return runSolutionUrl ?? "";
				return Url.Action("RunSolution", "Exercise", new { CourseId, SlideIndex, IsLti });
			}
			set { runSolutionUrl = value; }
		}

		public bool IsLti { get; set; }
		public bool DebugView { get; set; }
		public bool IsSkippedOrPassed { get; set; }
		public string CourseId { get; set; }
		public int SlideIndex { get; set; }

		public ExerciseReviewState ReviewState { get; set; }
		public List<ExerciseCodeReview> Reviews { get; set; }
	}

	public enum ExerciseReviewState
	{
		NotReviewed,
		WaitingForReview,
		Reviewed
	}
}