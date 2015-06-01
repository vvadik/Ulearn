namespace uLearn.Model
{
	public class BuildUpContext
	{
		public IFileSystem FileSystem { get; private set; }
		public CourseSettings CourseSettings { get; private set; }
		public Lesson Lesson { get; private set; }

		public BuildUpContext(IFileSystem fileSystem, CourseSettings courseSettings, Lesson lesson)
		{
			FileSystem = fileSystem;
			CourseSettings = courseSettings;
			Lesson = lesson;
		}
	}
}