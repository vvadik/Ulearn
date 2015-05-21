using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace uLearn
{
	public class CourseManager
	{
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

		public string GetStagingPackagePath(string name)
		{
			if (Path.GetInvalidFileNameChars().Any(name.Contains)) 
				throw new Exception(name);
			return StagedDirectory.GetFile(name).FullName;
		}

		private static readonly object ReloadLock = new object();
		
		private void LoadCoursesIfNotYet()
		{
			lock (ReloadLock)
			{
				if (courses.Count != 0) return;
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

		public void ReloadCourse(string packageName)
		{
			var file = StagedDirectory.GetFile(packageName);
			ReloadCourseFromZip(file);
		}

		private void ReloadCourseFromZip(FileInfo zipFile)
		{
			using (var zip = ZipFile.Read(zipFile.FullName, new ReadOptions {Encoding = Encoding.GetEncoding(866)}))
			{
				var courseId = Path.GetFileNameWithoutExtension(zipFile.Name);
				var courseDir = coursesDirectory.CreateSubdirectory(courseId);
				Directory.Delete(courseDir.FullName, true);
				courseDir.Create();
				zip.ExtractAll(courseDir.FullName, ExtractExistingFileAction.OverwriteSilently);
				ReloadCourse(courseDir);
			}
		}

		public void ReloadCourse(DirectoryInfo dir)
		{
			var course = loader.LoadCourse(dir);
			courses[course.Id] = course;
		}
		
	}
}