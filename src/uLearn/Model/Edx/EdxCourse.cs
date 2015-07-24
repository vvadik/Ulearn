using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ApprovalUtilities.Utilities;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	[XmlRoot("course")]
	public class EdxCourse
	{
		[XmlAttribute("url_name")]
		public string UrlName { get; set; }

		[XmlAttribute("org")]
		public string Organization;

		[XmlAttribute("course")]
		public string CourseName;

		[XmlIgnore]
		public string[] StaticFiles;

		[XmlIgnore]
		public CourseWithChapters CourseWithChapters;

		public EdxCourse()
		{
		}

		public EdxCourse(string courseId, string organization, string courseTitle, string[] advancedModules, string[] ltiPassports, Chapter[] chapters)
		{
			CourseName = courseId;
			UrlName = courseId;
			Organization = organization;
			CourseWithChapters = new CourseWithChapters(courseId, courseTitle, advancedModules, ltiPassports, true, chapters);
			StaticFiles = Directory.GetFiles("static");
		}

		private void CreateDirectories(string rootDir, params string[] subDirs)
		{
			if (Directory.Exists(rootDir))
				Directory.Delete(rootDir, true);
			Directory.CreateDirectory(rootDir);
			foreach (var subDir in subDirs)
				Directory.CreateDirectory(string.Format("{0}/{1}", rootDir, subDir));
		}

		public void Save(string folderName)
		{
			CreateDirectories(folderName, "course", "chapter", "sequential", "vertical", "video", "html", "lti", "static", "problem");
			foreach (var file in StaticFiles)
				File.Copy(file, string.Format("{0}/static/{1}", folderName, Path.GetFileName(file)));
			File.WriteAllText(string.Format("{0}/course.xml", folderName), this.XmlSerialize());
			CourseWithChapters.Save(folderName);
		}

		public static EdxCourse Load(string folderName)
		{
			var course = new FileInfo(string.Format("{0}/course.xml", folderName)).DeserializeXml<EdxCourse>();
			course.StaticFiles = Directory.GetFiles(string.Format("{0}/static", folderName));
			course.CourseWithChapters = CourseWithChapters.Load(folderName, course.UrlName);
			return course;
		}

		public void CreateTestChapter(string folderName, Vertical[] verticals)
		{
			if (CourseWithChapters.Chapters.All(x => x.UrlName != "TestChapter"))
			{
				var chapters = new List<Chapter>(CourseWithChapters.Chapters);
				var newChapter = new Chapter("TestChapter", "Test Chapter", new[] { new Sequential(Guid.NewGuid().ToString("D"), "Test Sequential", verticals) });
				chapters.Add(newChapter);
				CourseWithChapters.Chapters = chapters.ToArray();
				CourseWithChapters.ChapterReferences = CourseWithChapters.Chapters.Select(x => new ChapterReference { UrlName = x.UrlName }).ToArray();

				File.WriteAllText(string.Format("{0}/course/{1}.xml", folderName, CourseWithChapters.UrlName), CourseWithChapters.XmlSerialize());
				newChapter.Save(folderName);
			}
			else
			{
				var testChapter = CourseWithChapters.Chapters.Single(x => x.UrlName == "TestChapter");
				var sequentials = new List<Sequential>(testChapter.Sequentials);
				var newSequential = new Sequential(Guid.NewGuid().ToString("D"), "Test Sequential", verticals);
				sequentials.Add(newSequential);
				testChapter.Sequentials = sequentials.ToArray();
				testChapter.SequentialReferences = testChapter.Sequentials.Select(x => new SequentialReference { UrlName = x.UrlName }).ToArray();

				File.WriteAllText(string.Format("{0}/chapter/{1}.xml", folderName, testChapter.UrlName), testChapter.XmlSerialize());
				newSequential.Save(folderName);
			}
		}

		public void PatchVideos(string folderName, Dictionary<string, string> videoIds)
		{
			var newVideos = new List<VideoComponent>();
			foreach (var videoGuid in videoIds.Keys)
			{
				if (File.Exists(string.Format("{0}/video/{1}.xml", folderName, videoGuid)))
					new VideoComponent(videoGuid, new FileInfo(string.Format("{0}/video/{1}.xml", folderName, videoGuid)).DeserializeXml<VideoComponent>().DisplayName, videoIds[videoGuid]).Save(folderName);
				else 
					newVideos.Add(new VideoComponent(videoGuid, "", videoIds[videoGuid]));
			}
			if (newVideos.Count != 0)
			{
				var verticals = newVideos.Select(x => new Vertical(Guid.NewGuid().ToString("D"), x.DisplayName, new Component[] { x })).ToArray();
				CreateTestChapter(folderName, verticals);
			}
		}

		public void PatchSlides(string folderName, Slide[] slides)
		{
			var verticals = new List<Vertical>();
			foreach (var slide in slides)
			{
				var slideVerticals = slide.ToVerticals(folderName, "https://192.168.33.1:44300/Course/{0}/LtiSlide/", "https://192.168.33.1:44300/Course/{0}/AcceptedAlert/", new Dictionary<string, string>());
				if (File.Exists(string.Format("{0}/vertical/{1}.xml", folderName, slide.Guid)))
				{
					slideVerticals.ForEach(x => x.Save(folderName));
				}
				else
					verticals.AddRange(slideVerticals);
			}
			if (verticals.Count != 0)
			{
				CreateTestChapter(folderName, verticals.ToArray());
			}
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
