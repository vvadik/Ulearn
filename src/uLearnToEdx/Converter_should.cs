using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalUtilities.Utilities;
using NUnit.Framework;
using uLearn;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	[TestFixture]
	public class Converter_should
	{
		private Course course;
		private readonly Slide aTextSlide = new Slide(new[] { new MdBlock("hello"), }, new SlideInfo("u1", new FileInfo("file"), 0), "title", Guid.NewGuid().ToString("D"));
		private const string youtubeIdFromCourse = "GZS36w_fxdg";
		private const string slideIdFromCourse = "108C89D9-36F0-45E3-BBEE-B93AC971063F";


		[SetUp]
		public void SetUp()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			course = cm.GetCourses().Single();
		}

		private EdxCourse ConvertForTestsCourseToEdx(Dictionary<string, string> youtubeId2UlearnVideoIds = null)
		{
			return Converter.ToEdxCourse(course, "org", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "host",
				youtubeId2UlearnVideoIds ?? new Dictionary<string, string>());
		}

		public HashSet<string> GetDirectoryFiles(string directory)
		{
			var hs = new HashSet<string>();
			var files = Directory.GetFiles(directory).Select(Path.GetFileName);
			var dirFiles = Directory.GetDirectories(directory).SelectMany(x => Directory.GetFiles(x).Select(Path.GetFileName));
			hs.AddAll(files);
			hs.AddAll(dirFiles);
			return hs;
		}

		[Test]
		public void convert_saveAndLoadEdxCourse()
		{
			var edxCourse = ConvertForTestsCourseToEdx();

			edxCourse.Save("test/folder1");
			EdxCourse.Load("test/folder1").Save("test/folder2");

			CollectionAssert.AreEqual(GetDirectoryFiles("test/folder1"), GetDirectoryFiles("test/folder2"));
		}

		[Test]
		public void convert_assign_SlideIds_to_EdxUrlNames()
		{
			var edxCourse = ConvertForTestsCourseToEdx();

			var ulearnSlideIds = course.Slides.Select(x => x.Guid);
			var edxUrlNames = edxCourse.CourseWithChapters.Chapters[0].Sequentials.SelectMany(x => x.Verticals)
				.Where(x => x.DisplayName != "Решения")
				.Select(x => x.UrlName);
			CollectionAssert.AreEquivalent(ulearnSlideIds, edxUrlNames);
		}

		[Test]
		public void convert_assign_VideoIds_accordingToPassedDictionary()
		{
			var ulearnVideoGuid = Guid.NewGuid().ToString("D");
			var edxCourse = ConvertForTestsCourseToEdx(new Dictionary<string, string> { { youtubeIdFromCourse, ulearnVideoGuid } });
			Assert.AreEqual(youtubeIdFromCourse, edxCourse.GetVideoById(ulearnVideoGuid).NormalSpeedVideoId);
		}

		[Test]
		public void patch_updates_YoutubeIds()
		{
			var ulearnVideoGuid = Guid.NewGuid().ToString("D");
			var edxCourse = ConvertForTestsCourseToEdx(new Dictionary<string, string> { { youtubeIdFromCourse, ulearnVideoGuid } });
			edxCourse.Save("ForTests");
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { ulearnVideoGuid, "QWFuk3ymXxc" } });
			Assert.That(File.ReadAllText(string.Format("{0}/video/{1}.xml", "ForTests", ulearnVideoGuid)).Contains("QWFuk3ymXxc"));
		}

		[Test]
		public void patch_putsNewVideos_toExistingUnsortedChapter()
		{
			//TODO переделать тест согласно названию
			var videoGuid = Guid.NewGuid().ToString("D");
			var videoGuid2 = Guid.NewGuid().ToString("D");
			var edxCourse = ConvertForTestsCourseToEdx(new Dictionary<string, string> { { youtubeIdFromCourse, videoGuid } });
			edxCourse.Save("ForTests");
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { videoGuid, "QWFuk3ymXxc" }, { videoGuid2, "w8_GlqSkG-U" } });
			edxCourse.PatchVideos("ForTests", new Dictionary<string, string> { { videoGuid, "QWFuk3ymXxc" }, { videoGuid2, "w8_GlqSkG-U" }, { Guid.NewGuid().ToString("D"), "qTnKi67AAlg" } });
			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.AreEqual("Unsorted", edxCourse2.CourseWithChapters.Chapters[1].DisplayName);
			Assert.AreEqual(2, edxCourse2.CourseWithChapters.Chapters[1].Sequentials.Length);
		}

		[Test]
		public void patch_doesNotCreateUnsortedChapter_ifNoNewSlides()
		{
			var edxCourse = ConvertForTestsCourseToEdx();
			edxCourse.Save("ForTests");

			edxCourse.PatchSlides("ForTests", course.Slides);

			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.IsFalse(edxCourse2.CourseWithChapters.Chapters.Any(c => c.DisplayName == "Unsorted"));
		}

		[Test]
		public void patch_createsUnsortedChapter_withNewSlides()
		{
			var edxCourse = ConvertForTestsCourseToEdx();

			edxCourse.PatchSlides("ForTests", new[] { aTextSlide });

			var edxCourse2 = EdxCourse.Load("ForTests");
			Assert.AreEqual("Unsorted", edxCourse2.CourseWithChapters.Chapters.Last().DisplayName);
		}


		[Test]
		public void patch_updatesOrdinarySlide_withExerciseSlide()
		{
			var edxCourse = ConvertForTestsCourseToEdx();
			var slidesCount = edxCourse.CourseWithChapters.Chapters[0].Sequentials[0].Verticals.Count();
			edxCourse.PatchSlides("ForTests", new[] { new Slide(new[] { new ExerciseBlock() }, new SlideInfo("u1", new FileInfo("file"), 0), "title", Guid.Parse(slideIdFromCourse).ToString("D")) });
			var edxCourse2 = EdxCourse.Load("ForTests");
			var patchedSlidesCount = edxCourse2.CourseWithChapters.Chapters[0].Sequentials[0].Verticals.Count();
			Assert.AreEqual(slidesCount + 1, patchedSlidesCount);
		}
	}
}
