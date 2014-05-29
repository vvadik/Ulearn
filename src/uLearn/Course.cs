namespace uLearn
{
	public class Course
	{
		public Course(string id, string title, Slide[] slides)
		{
			Id = id;
			Title = title;
			Slides = slides;
		}

		public string Id { get; private set; }
		public string Title { get; private set; }
		public Slide[] Slides { get; private set; }
	}
}