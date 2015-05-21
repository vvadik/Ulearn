namespace uLearn.Web.Models
{
	public class ExerciseBlockData
	{
		public ExerciseBlockData(bool showControls = false, bool isSkipped = true, string latestAcceptedSolution = null)
		{
			CanSkip = !isSkipped && LatestAcceptedSolution == null;
			ShowControls = showControls;
			LatestAcceptedSolution = latestAcceptedSolution;
		}

		public bool ShowControls { get; private set; }
		public bool CanSkip { get; private set; }
		public string LatestAcceptedSolution { get; private set; }
		public string RunSolutionUrl { get; private set; } // Url.Action("RunSolution", "Exercise", new {courseId = context.Course.Id, slideIndex = context.Slide.Index})
		public string AcceptedSolutionUrl { get; private set; } // Url.Action("AcceptedSolutions", "Course", new { courseId = context.Course.Id, slideIndex = context.Slide.Index });
		public string GetHintUrl { get; private set; } // Url.Action("UseHint", "Hint")
	}
}