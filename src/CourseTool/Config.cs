using System;
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
	}

	public class Profile
	{
		[XmlAttribute("name")]
		public string Name;

		public string EdxStudioUrl;
		public string UlearnUrl;

		public static Profile GetProfile(Config config, string profile)
		{
			try
			{
				return config.Profiles.Single(x => x.Name == profile);
			}
			catch (Exception)
			{
				Console.WriteLine("Profiles were set up incorrectly");
				throw new OperationFailedGracefully();
			}
		}
	}
}
