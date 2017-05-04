using Database.Models;

namespace uLearn.Web.Models
{
	public class ExerciseCodeReviewModel
	{
		public ExerciseCodeReview Review { get; set; }
		public ManualExerciseChecking ManualChecking { get; set; }
	}
}