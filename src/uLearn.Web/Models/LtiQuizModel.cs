using uLearn.Quizes;

namespace uLearn.Web.Models
{
	public class LtiQuizModel
	{
		public QuizSlide Slide { get; set; }
		public string CourseId { get; set; }
		public string UserId { get; set; }
	}
}