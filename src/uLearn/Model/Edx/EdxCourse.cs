using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	[XmlRoot("course")]
	public class EdxCourse
	{
		[XmlAttribute("url_name")]
		public string UrlName { get; set; }

		[XmlIgnore]
		public string FolderName;

		[XmlAttribute("org")]
		public string Organization;

		[XmlAttribute("course")]
		public string CourseName;

		[XmlIgnore]
		public CourseWithChapters CourseWithChapters;

		public EdxCourse()
		{
		}

		public EdxCourse(string courseId, string organization, string courseTitle, string[] advancedModules, string[] ltiPassports, Chapter[] chapters)
		{
			CourseName = courseId;
			FolderName = courseId;
			UrlName = courseId;
			Organization = organization;
			CourseWithChapters = new CourseWithChapters(courseId, courseTitle, advancedModules, ltiPassports, true, chapters);
		}

		private void CreateDirectories(string rootDir, params string[] subDirs)
		{
			if (Directory.Exists(rootDir))
				Directory.Delete(rootDir, true);
			Directory.CreateDirectory(rootDir);
			foreach (var subDir in subDirs)
				Directory.CreateDirectory(string.Format("{0}/{1}", rootDir, subDir));
		}

		public void Save()
		{
			CreateDirectories(FolderName, "course", "chapter", "sequential", "vertical", "video", "html", "lti", "static", "problem");
			foreach (var file in Directory.GetFiles("static"))
				File.Copy(file, string.Format("{0}/static/{1}", FolderName, Path.GetFileName(file)));
			File.WriteAllText(string.Format("{0}/course.xml", FolderName), this.XmlSerialize());
			SaveAdditional();
		}

		private void SaveAdditional()
		{
			CourseWithChapters.Save(FolderName);
		}

		public Vertical GetVerticalById(string id)
		{
			return CourseWithChapters
				.Chapters
				.SelectMany(x => x.Sequentials.SelectMany(y => y.Verticals))
				.FirstOrDefault(x => x.UrlName == id);
		}

		public VideoComponent GetVideoById(string id)
		{
			return CourseWithChapters
				.Chapters
				.SelectMany(x => x.Sequentials.SelectMany(y => y.Verticals.SelectMany(z => z.Components)))
				.OfType<VideoComponent>()
				.FirstOrDefault(x => x.UrlName == id);
		}
	}
}
