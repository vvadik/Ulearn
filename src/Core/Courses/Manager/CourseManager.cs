using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ionic.Zip;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Units;
using Ulearn.Core.Helpers;
using Ulearn.Core.Telegram;
using Vostok.Logging.Abstractions;

namespace Ulearn.Core.Courses.Manager
{
	public abstract class CourseManager
	{
		private const string examplePackageName = "Help";

		private static ILog log => LogProvider.Get().ForContext(typeof(CourseManager));

		private static readonly CourseStorage courseStorage = new CourseStorage();
		public static ICourseStorage CourseStorageInstance => courseStorage;

		private readonly DirectoryInfo stagedDirectory;
		private readonly DirectoryInfo coursesDirectory;
		private readonly DirectoryInfo tempCourseStaging;
		private readonly DirectoryInfo coursesVersionsDirectory;

		/* LRU-cache for course versions. 50 is a capactiy of the cache. */
		private readonly LruCache<Guid, Course> versionsCache = new LruCache<Guid, Course>(50);

		private readonly ExerciseStudentZipsCache exerciseStudentZipsCache = new ExerciseStudentZipsCache(Options.Create(ApplicationConfiguration.Read<UlearnConfiguration>()));

		/* TODO (andgein): Use DI */
		private static readonly CourseLoader loader = new CourseLoader(new UnitLoader(new XmlSlideLoader()));
		private static readonly ErrorsBot errorsBot = new ErrorsBot();

		private static readonly ConcurrentDictionary<string, bool> courseIdToIsBroken = new ConcurrentDictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);

		public CourseManager(DirectoryInfo baseDirectory)
			: this(
				baseDirectory.GetSubdirectory("Courses.Staging"),
				baseDirectory.GetSubdirectory("Courses.Versions"),
				baseDirectory.GetSubdirectory("Courses"),
				baseDirectory.GetSubdirectory("TempCourseStaging")
			)
		{
		}

		private CourseManager(DirectoryInfo stagedDirectory, DirectoryInfo coursesVersionsDirectory, DirectoryInfo coursesDirectory, DirectoryInfo tempCourseStaging)
		{
			this.stagedDirectory = stagedDirectory;
			stagedDirectory.EnsureExists();
			this.coursesDirectory = coursesDirectory;
			coursesVersionsDirectory.EnsureExists();
			this.tempCourseStaging = tempCourseStaging;
			tempCourseStaging.EnsureExists();
			this.coursesVersionsDirectory = coursesVersionsDirectory;
			coursesVersionsDirectory.EnsureExists();
		}
		
		protected bool CourseIsBroken(string courseId)
		{
			return courseIdToIsBroken.TryGetValue(courseId, out var val) ? val : false;
		}

		protected void RemoveCourseFromBroken(string courseId)
		{
			courseIdToIsBroken.TryRemove(courseId, out _);
		}

		///
		/// <exception cref="CourseLoadingException"></exception>
		///
		public Course GetVersion(Guid versionId)
		{
			if (versionsCache.TryGet(versionId, out var version))
				return version;

			var versionFile = GetCourseVersionFile(versionId);
			version = LoadCourseFromZip(versionFile);

			/* Add version to cache for fast loading next time */
			versionsCache.Add(versionId, version);
			return version;
		}

		public FileInfo GetStagingCourseFile(string courseId)
		{
			var packageName = GetPackageName(courseId);
			if (Path.GetInvalidFileNameChars().Any(packageName.Contains))
				throw new Exception(courseId);
			return stagedDirectory.GetFile(packageName);
		}

		public FileInfo GetStagingTempCourseFile(string courseId)
		{
			var packageName = GetPackageName(courseId);
			if (Path.GetInvalidFileNameChars().Any(packageName.Contains))
				throw new Exception(courseId);
			return tempCourseStaging.GetFile(packageName);
		}

		public DirectoryInfo GetExtractedCourseDirectory(string courseId)
		{
			return coursesDirectory.GetSubdirectory(courseId);
		}

		public DirectoryInfo GetExtractedVersionDirectory(Guid versionId)
		{
			return GetExtractedCourseDirectory(versionId.GetNormalizedGuid());
		}

		public FileInfo GetCourseVersionFile(Guid versionId)
		{
			var packageName = GetPackageName(versionId);
			return coursesVersionsDirectory.GetFile(packageName);
		}

		public string GetStagingCoursePath(string courseId)
		{
			return GetStagingCourseFile(courseId).FullName;
		}

		private static readonly object reloadLock = new object();

		public void LoadCoursesIfNotYet()
		{
			lock (reloadLock)
			{
				if (courseStorage.GetCourses().Count() != 0)
					return;
				var courseZips = stagedDirectory.GetFiles("*.zip");
				var courseIdsWithZips = courseZips.Select(z => GetCourseId(z.FullName));
				LoadCourseZipsToDiskFromExternalStorage(courseIdsWithZips).Wait();

				log.Info($"Загружаю курсы из {stagedDirectory.FullName}");
				courseZips = stagedDirectory.GetFiles("*.zip");
				foreach (var zipFile in courseZips)
				{
					log.Info($"Обновляю курс из {zipFile.Name}");
					var courseId = GetCourseId(zipFile.FullName);
					TryReloadCourse(courseId);
				}
			}
		}

		protected virtual Task LoadCourseZipsToDiskFromExternalStorage(IEnumerable<string> existingOnDiskCourseIds)
		{
			return Task.CompletedTask;
		}

		public bool TryReloadCourse(string courseId)
		{
			if (CourseIsBroken(courseId))
				return false;
			try
			{
				ReloadCourseNotSafe(courseId);
				return true;
			}
			catch (Exception e)
			{
				log.Error(e, $"Не могу загрузить курс {courseId}");
				courseIdToIsBroken.AddOrUpdate(courseId, _ => true, (_, _) => true);
			}

			return false;
		}

		public void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true)
		{
			/* First try load course from directory */
			var courseDir = GetExtractedCourseDirectory(courseId);
			try
			{
				log.Info($"Сначала попробую загрузить уже распакованный курс из {courseDir.FullName}");
				ReloadCourseFromDirectory(courseDir);
			}
			catch (Exception e)
			{
				log.Warn(e, $"Не смог загрузить курс из папки {courseDir}");
				var zipFile = GetStagingCourseFile(courseId);
				log.Info($"Буду загружать из zip-архива: {zipFile.FullName}");
				if (notifyAboutErrors)
					errorsBot.PostToChannel($"Не смог загрузить курс из папки {courseDir}, буду загружать из zip-архива {zipFile.FullName}:\n{e.Message.EscapeMarkdown()}\n```{e.StackTrace}```", ParseMode.Markdown);
				ReloadCourseFromZip(zipFile);
			}
		}

		private Course ReloadCourseFromZip(FileInfo zipFile)
		{
			var course = LoadCourseFromZip(zipFile);
			courseStorage.AddOrUpdateCourse(course);
			log.Info($"Курс {course.Id} загружен из {zipFile.FullName} и сохранён в памяти");
			exerciseStudentZipsCache.DeleteCourseZips(course.Id);
			ExerciseCheckerZipsCache.DeleteCourseZips(course.Id);
			return course;
		}

		private Course ReloadCourseFromDirectory(DirectoryInfo directory)
		{
			var course = LoadCourseFromDirectory(directory);
			courseStorage.AddOrUpdateCourse(course);
			log.Info($"Курс {course.Id} загружен из {directory.FullName} и сохранён в памяти");
			exerciseStudentZipsCache.DeleteCourseZips(course.Id);
			ExerciseCheckerZipsCache.DeleteCourseZips(course.Id);
			return course;
		}

		protected void UnzipFile(FileInfo zipFile, DirectoryInfo unpackDirectory)
		{
			using (var zip = ZipFile.Read(zipFile.FullName, new ReadOptions { Encoding = ZipUtils.Cp866 })) // Использует UTF8, где нужно
			{
				log.Info($"Очищаю директорию {unpackDirectory.FullName}");
				unpackDirectory.ClearDirectory();
				log.Info($"Директория {unpackDirectory.FullName} очищена");
				zip.ExtractAll(unpackDirectory.FullName, ExtractExistingFileAction.OverwriteSilently);
				foreach (var f in unpackDirectory.GetFiles("*", SearchOption.AllDirectories).Cast<FileSystemInfo>().Concat(unpackDirectory.GetDirectories("*", SearchOption.AllDirectories)))
					f.Attributes &= ~FileAttributes.ReadOnly;
				log.Info($"Архив {zipFile.FullName} распакован");
			}
		}

		private void UnzipTempWithOverwrite(FileInfo zipFile, DirectoryInfo unpackDirectory)
		{
			using (var zip = ZipFile.Read(zipFile.FullName, new ReadOptions { Encoding = Encoding.UTF8 }))
			{
				zip.ExtractAll(unpackDirectory.FullName, ExtractExistingFileAction.OverwriteSilently);
				foreach (var f in unpackDirectory.GetFiles("*", SearchOption.AllDirectories).Cast<FileSystemInfo>().Concat(unpackDirectory.GetDirectories("*", SearchOption.AllDirectories)))
					f.Attributes &= ~FileAttributes.ReadOnly;
				log.Info($"Архив {zipFile.FullName} распакован");
			}
		}

		private DirectoryInfo UnzipCourseFile(FileInfo zipFile)
		{
			var courseOrVersionId = GetCourseId(zipFile.Name);
			var courseDir = coursesDirectory.GetSubdirectory(courseOrVersionId);
			if (courseDir.Exists)
				return courseDir;
			courseDir.Create();
			log.Info($"Распаковываю архив с курсом из {zipFile.FullName} в {courseDir.FullName}");
			UnzipFile(zipFile, courseDir);
			log.Info($"Распаковал архив с курсом из {zipFile.FullName} в {courseDir.FullName}");
			return courseDir;
		}

		private Course LoadCourseFromZip(FileInfo zipFile)
		{
			var courseDir = UnzipCourseFile(zipFile);
			return LoadCourseFromDirectory(courseDir);
		}

		private Course LoadCourseFromDirectory(DirectoryInfo dir)
		{
			WaitWhileCourseIsLocked(GetCourseId(dir.Name));
			return loader.Load(dir);
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
			return versionId.GetNormalizedGuid() + ".zip";
		}

		public bool TryCreateCourse(string courseId, string courseTitle, Guid firstVersionId)
		{
			try
			{
				if (courseId.Any(GetInvalidCharacters().Contains))
					return false;

				var package = stagedDirectory.GetFile(GetPackageName(courseId));
				if (package.Exists)
					return true;

				var examplePackage = stagedDirectory.GetFile(GetPackageName(examplePackageName));
				if (!examplePackage.Exists)
					CreateEmptyCourse(courseId, courseTitle, package.FullName);
				else
					CreateCourseFromExample(courseId, courseTitle, package.FullName, examplePackage);

				ReloadCourseFromZip(package);

				var versionFile = GetCourseVersionFile(firstVersionId);
				File.Copy(package.FullName, versionFile.FullName);

				return true;
			}
			catch (Exception ex)
			{
				log.Error(ex, $"Error on create course {courseId}");
				courseIdToIsBroken.AddOrUpdate(courseId, _ => true, (_, _) => true);
				return false;
			}
		}

		public bool TryCreateTempCourse(string courseId, string courseTitle, Guid firstVersionId)
		{
			try
			{
				//todo дубликат метода TryCreateCourse. Можно убрать создание пустой версии и пустого архива в Staging
				if (courseId.Any(GetInvalidCharacters().Contains))
					return false;

				var package = stagedDirectory.GetFile(GetPackageName(courseId));
				if (package.Exists)
					return true;

				var examplePackage = stagedDirectory.GetFile(GetPackageName(examplePackageName));
				if (!examplePackage.Exists)
					CreateEmptyCourse(courseId, courseTitle, package.FullName);
				else
					CreateCourseFromExample(courseId, courseTitle, package.FullName, examplePackage);

				ReloadCourseFromZip(package);

				var versionFile = GetCourseVersionFile(firstVersionId);
				File.Copy(package.FullName, versionFile.FullName);

				return true;
			}
			catch (Exception ex)
			{
				log.Error(ex, $"Error on create temp course {courseId}");
				courseIdToIsBroken.AddOrUpdate(courseId.ToLower(), _ => true, (_, _) => true);
				return false;
			}
		}

		public void EnsureVersionIsExtracted(Guid versionId)
		{
			var versionDirectory = GetExtractedVersionDirectory(versionId);
			if (!versionDirectory.Exists)
			{
				Directory.CreateDirectory(versionDirectory.FullName);
				UnzipFile(GetCourseVersionFile(versionId), versionDirectory);
			}
		}

		public void ExtractTempCourseChanges(string tempCourseId)
		{
			var zipWithChanges = GetStagingTempCourseFile(tempCourseId);
			var courseDirectory = GetExtractedCourseDirectory(tempCourseId);
			UnzipTempWithOverwrite(zipWithChanges, courseDirectory);
		}

		private static void CreateEmptyCourse(string courseId, string courseTitle, string path)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				zip.AddEntry("course.xml",
					"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
					$"<course xmlns=\"https://ulearn.me/schema/v2\" title=\"{courseTitle.EncodeQuotes()}\">\n" +
					@"<units><add>*\unit.xml</add></units>" +
					"</course>",
					new UTF8Encoding(false));
				zip.Save(path);
			}
		}

		private static void CreateCourseFromExample(string courseId, string courseTitle, string path, FileInfo examplePackage)
		{
			examplePackage.CopyTo(path, true);
			File.SetLastWriteTime(examplePackage.FullName, DateTime.Now);

			var nsResolver = new XmlNamespaceManager(new NameTable());
			nsResolver.AddNamespace("ulearn", "https://ulearn.me/schema/v2");
			using (var zip = ZipFile.Read(path, new ReadOptions { Encoding = ZipUtils.Cp866 }))
			{
				var courseXml = zip.Entries.FirstOrDefault(e => Path.GetFileName(e.FileName).Equals("course.xml", StringComparison.OrdinalIgnoreCase) && !e.IsDirectory);
				if (courseXml != null)
					UpdateXmlAttribute(zip[courseXml.FileName], "//ulearn:course", "title", courseTitle, zip, nsResolver);
			}
		}

		private static void UpdateXmlAttribute(ZipEntry entry, string selector, string attribute, string value, ZipFile zip, IXmlNamespaceResolver nsResolver)
		{
			UpdateXmlEntity(entry, selector, element =>
			{
				var elementAttribute = element.Attribute(attribute);
				if (elementAttribute != null)
					elementAttribute.Value = value;
			}, zip, nsResolver);
		}

		private static void UpdateXmlEntity(ZipEntry entry, string selector, Action<XElement> update, ZipFile zip, IXmlNamespaceResolver nsResolver)
		{
			using (var output = StaticRecyclableMemoryStreamManager.Manager.GetStream())
			{
				using (var entryStream = entry.OpenReader())
				{
					var xml = XDocument.Load(entryStream);
					var element = xml.XPathSelectElement(selector, nsResolver);
					update(element.EnsureNotNull($"no element [{selector}] in zip entry {entry.FileName}"));
					xml.Save(output);
				}

				output.Position = 0;
				zip.UpdateEntry(entry.FileName, output.ToArray());
				zip.Save();
			}
		}

		public bool HasPackageFor(string courseId)
		{
			return GetStagingCourseFile(courseId).Exists;
		}

		public static char[] GetInvalidCharacters()
		{
			return new[] { '&' }.Concat(Path.GetInvalidFileNameChars()).Concat(Path.GetInvalidPathChars()).Distinct().ToArray();
		}

		private void UpdateCourse(Course course)
		{
			if (!courseStorage.HasCourse(course.Id))
				return;

			exerciseStudentZipsCache.DeleteCourseZips(course.Id);
			ExerciseCheckerZipsCache.DeleteCourseZips(course.Id);

			var versionId = Guid.Empty;
			courseStorage.AddOrUpdateCourse(course);
		}

		private readonly TimeSpan waitBetweenLockTries = TimeSpan.FromSeconds(0.1);
		private readonly TimeSpan lockLifeTime = TimeSpan.FromMinutes(20);
		private const int updateCourseEachOperationTriesCount = 5;

		private FileInfo GetCourseLockFile(string courseId)
		{
			return coursesDirectory.GetFile("~" + courseId + ".lock");
		}

		private static bool TryCreateLockFile(FileInfo lockFile)
		{
			var tempFileName = Path.GetTempFileName();
			try
			{
				if (!lockFile.Directory.Exists)
					lockFile.Directory.Create();
				new FileInfo(tempFileName).MoveTo(lockFile.FullName);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		private void LockCourse(string courseId)
		{
			var lockFile = GetCourseLockFile(courseId);
			while (true)
			{
				if (TryCreateLockFile(lockFile))
					return;

				log.Info($"Курс {courseId} заблокирован, жду {waitBetweenLockTries.TotalSeconds} секунд");

				Thread.Sleep(waitBetweenLockTries);

				try
				{
					lockFile.Refresh();
					/* If lock-file has been created ago, just delete it and unzip course again */
					if (lockFile.Exists && lockFile.LastWriteTime < DateTime.Now.Subtract(lockLifeTime))
					{
						log.Info($"Курс {courseId} заблокирован слишком давно, снимаю блокировку");

						lockFile.Delete();
						UnzipCourseFile(GetStagingCourseFile(courseId));
					}
				}
				catch (IOException)
				{
				}
			}
		}

		private void ReleaseCourse(string courseId)
		{
			GetCourseLockFile(courseId).Delete();
		}

		public void WaitWhileCourseIsLocked(string courseId)
		{
			LockCourse(courseId);
			log.Info($"Course is locked {courseId}");
			ReleaseCourse(courseId);
			log.Info($"Course lock released {courseId}");
		}

		public void MoveCourse(Course course, DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
		{
			var tempDirectoryName = coursesDirectory.GetSubdirectory(Path.GetRandomFileName());
			LockCourse(course.Id);

			try
			{
				FuncUtils.TrySeveralTimes(() => Directory.Move(destinationDirectory.FullName, tempDirectoryName.FullName), updateCourseEachOperationTriesCount);

				try
				{
					FuncUtils.TrySeveralTimes(() => Directory.Move(sourceDirectory.FullName, destinationDirectory.FullName), updateCourseEachOperationTriesCount);
				}
				catch (IOException)
				{
					/* In case of any file system's error rollback previous operation */
					FuncUtils.TrySeveralTimes(() => Directory.Move(tempDirectoryName.FullName, destinationDirectory.FullName), updateCourseEachOperationTriesCount);
					throw;
				}

				UpdateCourse(course);
			}
			finally
			{
				ReleaseCourse(course.Id);
			}

			FuncUtils.TrySeveralTimes(() => tempDirectoryName.ClearDirectory(true), updateCourseEachOperationTriesCount);
		}

		public static DirectoryInfo GetCoursesDirectory()
		{
			var coursesDirectory = ApplicationConfiguration.Read<UlearnConfiguration>().CoursesDirectory ?? "";
			if (!Path.IsPathRooted(coursesDirectory))
				coursesDirectory = Path.Combine(Utils.GetAppPath(), coursesDirectory);

			return new DirectoryInfo(coursesDirectory);
		}

		public FileInfo GenerateOrFindStudentZip(string courseId, Slide slide)
		{
			return exerciseStudentZipsCache.GenerateOrFindZip(courseId, slide, GetExtractedCourseDirectory(courseId).FullName);
		}
	}
}