using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ionic.Zip;

namespace uLearn
{
	public class CourseManager
	{
		public const string HelpPackageName = "Help";

		private readonly DirectoryInfo coursesDirectory;
		private readonly Dictionary<string, Course> courses = new Dictionary<string, Course>(StringComparer.InvariantCultureIgnoreCase);
		private readonly CourseLoader loader = new CourseLoader();

		public CourseManager(DirectoryInfo baseDirectory)
			: this(
				baseDirectory.GetSubdir("Courses.Staging"),
				baseDirectory.GetSubdir("Courses"))
		{
		}

		public CourseManager(DirectoryInfo stagedDirectory, DirectoryInfo coursesDirectory)
		{
			StagedDirectory = stagedDirectory;
			this.coursesDirectory = coursesDirectory;
		}

		public DirectoryInfo StagedDirectory { get; private set; }

		public IEnumerable<Course> GetCourses()
		{
			LoadCoursesIfNotYet();
			return courses.Values;
		}

		public Course GetCourse(string courseId)
		{
			LoadCoursesIfNotYet();
			return courses.Get(courseId);
		}

		public IEnumerable<StagingPackage> GetStagingPackages()
		{
			return StagedDirectory.GetFiles("*.zip").Select(f => new StagingPackage(f.Name, f.LastWriteTime));
		}

		public FileInfo GetStagingCourseFile(string courseId)
		{
			var packageName = GetPackageName(courseId);
			if (Path.GetInvalidFileNameChars().Any(packageName.Contains))
				throw new Exception(courseId);
			return StagedDirectory.GetFile(packageName);
		}

		public string GetStagingCoursePath(string courseId)
		{
			return GetStagingCourseFile(courseId).FullName;
		}

		private static readonly object ReloadLock = new object();

		private void LoadCoursesIfNotYet()
		{
			lock (ReloadLock)
			{
				if (courses.Count != 0)
					return;
				var courseZips = StagedDirectory.GetFiles("*.zip");
				foreach (var zipFile in courseZips)
					try
					{
						ReloadCourseFromZip(zipFile);
					}
					catch
					{
						//throw new Exception("Error loading course from " + zipFile.Name, e);
					}
			}
		}

		public string ReloadCourse(string courseId)
		{
			var file = StagedDirectory.GetFile(GetPackageName(courseId));
			return ReloadCourseFromZip(file);
		}

		private string ReloadCourseFromZip(FileInfo zipFile)
		{
			string courseId = "";
			using (var zip = ZipFile.Read(zipFile.FullName, new ReadOptions { Encoding = Encoding.GetEncoding(866) }))
			{
				courseId = GetCourseId(zipFile.Name);
				var courseDir = coursesDirectory.CreateSubdirectory(courseId);
				Directory.Delete(courseDir.FullName, true);
				courseDir.Create();
				zip.ExtractAll(courseDir.FullName, ExtractExistingFileAction.OverwriteSilently);
				ReloadCourse(courseDir);
			}
			return courseId;
		}

		public void ReloadCourse(DirectoryInfo dir)
		{
			var course = loader.LoadCourse(dir);
			courses[course.Id] = course;
		}

		public static string GetCourseId(string packageName)
		{
			return Path.GetFileNameWithoutExtension(packageName);
		}

		public string GetPackageName(string courseId)
		{
			return courseId + ".zip";
		}

		public DateTime GetLastWriteTime(string courseId)
		{
			return StagedDirectory.GetFile(GetPackageName(courseId)).LastWriteTime;
		}

		public bool TryCreateCourse(string courseId)
		{
			
			if (courseId.Any(GetInvalidCharacters().Contains))
				return false;

			var package = StagedDirectory.GetFile(GetPackageName(courseId));
			if (package.Exists)
				return true;

			var helpPackage = StagedDirectory.GetFile(GetPackageName(HelpPackageName));
			if (!helpPackage.Exists)
				CreateEmptyCourse(courseId, package.FullName);
			else
				CreateCourseFromExample(courseId, package.FullName, helpPackage);

			ReloadCourseFromZip(package);
			return true;
		}

		private static void CreateEmptyCourse(string courseId, string path)
		{
			using (var zip = new ZipFile(Encoding.GetEncoding(866)))
			{
				zip.AddEntry("Course.xml", 
					string.Format(
						"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
						"<Course xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"https://ulearn.azurewebsites.net/course\">\n" +
						"\t<title>{0}</title>\n" +
						"</Course>",
						courseId), 
					Encoding.UTF8);
				zip.Save(path);
			}
		}

		private static void CreateCourseFromExample(string courseId, string path, FileInfo helpPackage)
		{
			helpPackage.CopyTo(path, true);
			var nsResolver = new XmlNamespaceManager(new NameTable());
			nsResolver.AddNamespace("course", "https://ulearn.azurewebsites.net/course");
			nsResolver.AddNamespace("lesson", "https://ulearn.azurewebsites.net/lesson");
			nsResolver.AddNamespace("quiz", "https://ulearn.azurewebsites.net/quiz");
			using (var zip = ZipFile.Read(path, new ReadOptions { Encoding = Encoding.GetEncoding(866) }))
			{
				UpdateXmlElement(zip["Course.xml"], "//course:Course/course:title", courseId, zip, nsResolver);
				foreach (var entry in zip.SelectEntries("name = *.lesson.xml").Where(entry => CourseLoader.IsSlideFile(Path.GetFileName(entry.FileName))))
					UpdateXmlElement(entry, "//lesson:Lesson/lesson:id", Guid.NewGuid().ToString(), zip, nsResolver);
				foreach (var entry in zip.SelectEntries("name = *.quiz.xml").Where(entry => CourseLoader.IsSlideFile(Path.GetFileName(entry.FileName))))
					UpdateXmlAttribute(entry, "//quiz:Quiz", "id", Guid.NewGuid().ToString(), zip, nsResolver);
				foreach (var entry in zip.SelectEntries("name = *.cs").Where(entry => CourseLoader.IsSlideFile(Path.GetFileName(entry.FileName))))
					UpdateCsFiles(entry, Guid.NewGuid().ToString(), zip);
			}
		}

		private static void UpdateXmlElement(ZipEntry entry, string selector, string value, ZipFile zip, IXmlNamespaceResolver nsResolver)
		{
			UpdateXmlEntity(entry, selector, element => element.Value = value, zip, nsResolver);
		}

		private static void UpdateXmlAttribute(ZipEntry entry, string selector, string attribute, string value, ZipFile zip, XmlNamespaceManager nsResolver)
		{
			UpdateXmlEntity(entry, selector, element => element.Attribute(attribute).Value = value, zip, nsResolver);
		}

		private static void UpdateCsFiles(ZipEntry entry, string slideId, ZipFile zip)
		{
			string code;
			using (var entryStream = entry.OpenReader())
			{
				code = new StreamReader(entryStream).ReadToEnd();
			}
			code = Regex.Replace(code, "(?<=\\[Slide\\(\".*\",\\s*\").+(?=\"\\)\\])", slideId);
			zip.UpdateEntry(entry.FileName, code, Encoding.UTF8);
			zip.Save();
		}

		private static void UpdateXmlEntity(ZipEntry entry, string selector, Action<XElement> update, ZipFile zip, IXmlNamespaceResolver nsResolver)
		{
			var output = new MemoryStream();
			using (var entryStream = entry.OpenReader())
			{
				var xml = XDocument.Load(entryStream);
				update(xml.XPathSelectElement(selector, nsResolver));
				xml.Save(output);
			}
			zip.UpdateEntry(entry.FileName, output.GetBuffer());
			zip.Save();
		}

		public bool HasPackageFor(string courseId)
		{
			return GetStagingCourseFile(courseId).Exists;
		}

		public static char[] GetInvalidCharacters()
		{
			return new []{'&'}.Concat(Path.GetInvalidFileNameChars()).Concat(Path.GetInvalidPathChars()).Distinct().ToArray();
		}
	}
}