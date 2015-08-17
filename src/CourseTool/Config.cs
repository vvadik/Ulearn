using System.Xml.Serialization;

namespace uLearn.CourseTool
{
	public class Config
	{
		public string Hostname;
		public int Port;
		public string Organization;
		public string CourseNumber;
		public string CourseRun;
		public string ExerciseUrl;
		public string SolutionsUrl;
		public string LtiId;
		public string ULearnCourseId;
		public string Video;
		[XmlArrayItem("SlideId")]
		public string[] IgnoredSlides;
	}
}
