using Ulearn.Core.Courses;

namespace uLearn.Web.Models
{
	public class IntructorNoteModel
	{
		public IntructorNoteModel(string courseId, InstructorNote note)
		{
			CourseId = courseId;
			Note = note;
		}

		public string CourseId;
		public InstructorNote Note;
	}
}