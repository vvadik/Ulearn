namespace uLearn.Web.Models
{
	public class ExerciseBlockData
	{
		public ExerciseBlockData(bool showControls = true, bool isSkipped = true, string latestAcceptedSolution = null)
		{
			ShowControls = showControls;
			LatestAcceptedSolution = latestAcceptedSolution;
			CanSkip = !isSkipped && LatestAcceptedSolution == null;
		}

		public bool ShowControls { get; private set; }
		public bool CanSkip { get; private set; }
		public string LatestAcceptedSolution { get; private set; }
		public string RunSolutionUrl { get; set; }
		public string AcceptedSolutionUrl { get; set; }
		public string GetHintUrl { get; set; }
		
		public bool IsLti { get; set; }
		public bool ShowHints { get; set; }
		public bool IsSkippedOrPassed { get; set; }
		public string CourseId { get; set; }
		public int SlideIndex { get; set; }
	}
}