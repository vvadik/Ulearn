using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using NUnit.Framework;
using ObjectPrinter;
using uLearn;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	[TestFixture]
	public class Converter_should
	{
		//		[Test, UseReporter(typeof(DiffReporter))]
		//		public void save_files()
		//		{
		//			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
		//			cm.ReloadCourse("ForTests.zip");
		//			var course = cm.GetCourses().Single();
		//			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host");
		//			edxCourse.Save("folder");
		//			//			ObjectPrinter.ObjectExtensions.
		//			Approvals.Verify(edxCourse.Dump(new ObjectPrinterConfig() { MaxDepth = 11 }));
		//		}

		public HashSet<string> GetDirectoryFiles(string directory)
		{
			var hs = new HashSet<string>();
			var files = Directory.GetFiles(directory).Select(Path.GetFileName);
			var dirFiles = Directory.GetDirectories(directory).SelectMany(x => Directory.GetFiles(x).Select(y => Path.GetFileName(y)));
			hs.AddAll(files);
			hs.AddAll(dirFiles);
			return hs;
		}

		[Test]
		public void save_and_load()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host", new Dictionary<string, string>());
			edxCourse.Save("test/folder1");

			EdxCourse.Load("test/folder1").Save("test/folder2");
			CollectionAssert.AreEqual(GetDirectoryFiles("test/folder1"), GetDirectoryFiles("test/folder2"));
		}

		[Test]
		public void use_slide_guids()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host", new Dictionary<string, string>());
			CollectionAssert.AreEqual(
				new HashSet<string>(course.Slides.Select(x => x.Guid)),
				new HashSet<string>(edxCourse.CourseWithChapters.Chapters[0].Sequentials.SelectMany(x => x.Verticals).Where(x => x.DisplayName != "Решения").Select(x => x.UrlName))
				);
		}

		[Test]
		public void use_video_guids()
		{
			var videoId = "GZS36w_fxdg";
			var videoGuid = Guid.NewGuid().ToString("D");
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host", new Dictionary<string, string> { { videoId, videoGuid } });
			Assert.AreEqual(videoId, edxCourse.GetVideoById(videoGuid).VideoId1);
		}

		[Test]
		public void patch_videos()
		{
			var videoId = "GZS36w_fxdg";
			var videoGuid = Guid.NewGuid().ToString("D");
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host", new Dictionary<string, string> { { videoId, videoGuid } });
			edxCourse.Save("ForTests");
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { videoGuid, "QWFuk3ymXxc" } });
			Assert.That(File.ReadAllText(string.Format("{0}/video/{1}.xml", "ForTests", videoGuid)).Contains("QWFuk3ymXxc"));
		}
		
		[Test]
		public void create_new_chapter()
		{
			var videoId = "GZS36w_fxdg";
			var videoGuid = Guid.NewGuid().ToString("D");
			var videoGuid2 = Guid.NewGuid().ToString("D");
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "org", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "host", new Dictionary<string, string> { { videoId, videoGuid } });
			edxCourse.Save("ForTests");
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { videoGuid, "QWFuk3ymXxc" }, { videoGuid2, "w8_GlqSkG-U" } });
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { videoGuid, "QWFuk3ymXxc" }, { videoGuid2, "w8_GlqSkG-U" }, { Guid.NewGuid().ToString("D"), "qTnKi67AAlg" } });
			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.AreEqual(2, edxCourse2.CourseWithChapters.Chapters.Length);
			Assert.AreEqual(2, edxCourse2.CourseWithChapters.Chapters[1].Sequentials.Length);
		}

		[Test]
		public void patch_existing_slides()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();

			var edxCourse = Converter.ToEdxCourse(course, "org", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "host", new Dictionary<string, string>());
			edxCourse.Save("ForTests");
			edxCourse.PatchSlides("ForTests", course.Slides);

			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.AreEqual(1, edxCourse2.CourseWithChapters.Chapters.Length);
		}

		[Test]
		public void add_new_slides()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			
			var edxCourse = Converter.ToEdxCourse(course, "org", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "host", new Dictionary<string, string>());
			edxCourse.Save("ForTests");

			var cm2 = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm2.ReloadCourse("Help.zip");
			var course2 = cm2.GetCourses().Single();

			edxCourse.PatchSlides("ForTests", course2.Slides);

			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.AreEqual(2, edxCourse2.CourseWithChapters.Chapters.Length);
		}
	}
}
