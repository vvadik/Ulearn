using System.Linq;
using System.Xml.Serialization;

namespace uLearn.CourseTool
{
	public class Config
	{
		public string Organization;
		public string CourseNumber;
		public string CourseRun;
		public string LtiId;
		public string ULearnCourseId;
		public string Video;
		public Profile[] Profiles;

		[XmlArrayItem("SlideId")]
		public string[] IgnoredUlearnSlides = new string[0];

		public bool EmitSequentialsForInstructorNotes { get; set; }

		public Profile GetProfile(string profile)
		{
			return Profiles.SingleVerbose(x => x.Name == profile, "profile name = " + profile);
		}
	}

	public class Profile
	{
		[XmlAttribute("name")]
		public string Name;

		public string EdxStudioUrl;
		public string UlearnUrl;
	}
}