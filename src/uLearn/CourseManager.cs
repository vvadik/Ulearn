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

		private static readonly object reloadLock = new object();

		private void LoadCoursesIfNotYet()
		{
			Exception firstException = null;
			lock (reloadLock)
			{
				if (courses.Count != 0)
					return;
				var courseZips = StagedDirectory.GetFiles("*.zip");
				foreach (var zipFile in courseZips)
				{
					var courseOrVersionId = GetCourseId(zipFile.Name);
					/* Guid-named zips are not courses, they are course versions */
					Guid guid;
					if (Guid.TryParse(courseOrVersionId, out guid))
						continue;

					try
					{
						ReloadCourseFromZip(zipFile);
					}
					catch (Exception e)
					{
						firstException = new Exception("Error loading course from " + zipFile.Name, e);
					}
				}
			}
			if (firstException != null)
				throw firstException;
		}

		public Course ReloadCourse(string courseId)
		{
			var file = StagedDirectory.GetFile(GetPackageName(courseId));
			return ReloadCourseFromZip(file);
		}

		private Course ReloadCourseFromZip(FileInfo zipFile)
		{
			var course = LoadCourseFromZip(zipFile);
			courses[course.Id] = course;
			return course;
		}

		private void ClearDirectory(DirectoryInfo directory)
		{
			foreach (var file in directory.GetFiles())
				file.Delete();
			foreach (var subDirectory in directory.GetDirectories())
				subDirectory.Delete(true);
		}

		public Course LoadCourseFromZip(FileInfo zipFile)
		{
			using (var zip = ZipFile.Read(zipFile.FullName, new ReadOptions { Encoding = Encoding.GetEncoding(866) }))
			{
				var courseOrVersionId = GetCourseId(zipFile.Name);
				var courseDir = coursesDirectory.CreateSubdirectory(courseOrVersionId);
				ClearDirectory(courseDir);
				zip.ExtractAll(courseDir.FullName, ExtractExistingFileAction.OverwriteSilently);
				return LoadCourseFromDirectory(courseDir);
			}
		}

		public Course LoadCourseFromDirectory(DirectoryInfo dir)
		{
			return loader.LoadCourse(dir);
		}

		public static string GetCourseId(string packageName)
		{
			return Path.GetFileNameWithoutExtension(packageName);
		}

		public string GetPackageName(string courseId)
		{
			return courseId + ".zip";
		}

		public string GetPackageName(Guid versionId)
		{
			return Utils.GetNormalizedGuid(versionId) + ".zip";
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

		public Course FindCourseBySlideById(Guid slideId)
		{
			return GetCourses().FirstOrDefault(c => c.Slides.Count(s => s.Id == slideId) > 0);
		}

		public void UpdateCourse(Course course)
		{
			if (!courses.ContainsKey(course.Id))
				return;
			courses[course.Id] = course;
		}
	}
}