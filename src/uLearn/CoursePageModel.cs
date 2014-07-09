namespace uLearn
{
	public class CoursePageModel
	{
		public Course Course;
		public Slide Slide;
		public int SlideIndex;
		public bool HasNextSlide { get { return SlideIndex < Course.Slides.Length - 1; } }
		public bool HasPrevSlide { get { return SlideIndex > 0; } }

		public string SlideClass
		{
			get { return Slide is ExerciseSlide ? "exercise" : "theory"; }
		}

		public string ActualExerciseOutput { get; private set; }
		public string ExecutionVerdict { get; private set; }
		public ExecutionStatus ExecutionStatus { get; private set; }
	}

	public enum ExecutionStatus
	{
		// тест пройден
		OK,
		
		//неверный результат
		WA,

		//ошибка компиляции
		CompilationError,

		//запрещенное действие в коде
		SemanticError
	}
}