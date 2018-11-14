using System;
using uLearn.Courses.Slides.Blocks;

namespace uLearn.Web.Models
{
	public class CodeModel
	{
		public ExerciseBlock ExerciseBlock;
		public string CourseId;
		public BlockRenderContext Context;
		public Guid SlideId;
	}
}