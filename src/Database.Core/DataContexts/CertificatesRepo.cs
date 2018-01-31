using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Database.Models;
using Ionic.Zip;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using uLearn;
using uLearn.Configuration;
using uLearn.Extensions;
using Ulearn.Common.Extensions;

namespace Database.DataContexts
{
	public class CertificatesRepo
	{
		private readonly ULearnDb db;

		private readonly VisitsRepo visitsRepo;
		private readonly UserQuizzesRepo userQuizzesRepo;
		private readonly UserSolutionsRepo userSolutionsRepo;
		private readonly SlideCheckingsRepo slideCheckingsRepo;
		private readonly UlearnConfiguration configuration;

		private static readonly ILog log = LogManager.GetLogger(typeof(CertificatesRepo));

		public const string TemplateIndexFile = "index.html";
		private readonly Regex templateParameterRegex = new Regex(@"%([-a-z0-9_.]+)(\|(raw|in_quotes|in_html))?%", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private readonly HashSet<string> builtInParameters = new HashSet<string>
		{
			"user.last_name", "user.first_name", "user.name",
			"instructor.last_name", "instructor.first_name", "instructor.name",
			"course.id", "course.title",
			"date", "date.year", "date.month", "date.day",
			"certificate.id", "certificate.url",
			"score",
			"codereviews.passed", "codereviews.passed_maxscore",
			"quizzes.passed", "quizzes.passed_maxscore",
			"exercises.accepted",
		};

		public CertificatesRepo(
			ULearnDb db,
			VisitsRepo visitsRepo, UserQuizzesRepo userQuizzesRepo, UserSolutionsRepo userSolutionsRepo, SlideCheckingsRepo slideCheckingsRepo,
			IOptions<UlearnConfiguration> configuration)
		{
			this.db = db;
			this.visitsRepo = visitsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.userSolutionsRepo = userSolutionsRepo;
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.configuration = configuration.Value;
		}

		public List<CertificateTemplate> GetTemplates(string courseId)
		{
			return db.CertificateTemplates.Where(t => t.CourseId == courseId && !t.IsDeleted).ToList();
		}

		public CertificateTemplate FindTemplateById(Guid id)
		{
			return db.CertificateTemplates.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
		}

		public Certificate FindCertificateById(Guid id)
		{
			return db.Certificates.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
		}

		public List<Certificate> GetTemplateCertificates(Guid templateId)
		{
			return db.Certificates.Where(c => c.TemplateId == templateId && !c.IsDeleted).ToList();
		}

		public async Task<CertificateTemplate> AddTemplate(string courseId, string name, string archiveName)
		{
			var template = new CertificateTemplate
			{
				Id = Guid.NewGuid(),
				CourseId = courseId,
				Name = name,
				Timestamp = DateTime.Now,
				ArchiveName = archiveName,
			};
			db.CertificateTemplates.Add(template);
			await db.SaveChangesAsync();
			return template;
		}

		public async Task<Certificate> AddCertificate(Guid templateId, string userId, string instructorId, Dictionary<string, string> parameters, bool isPreview = false)
		{
			var certificate = new Certificate
			{
				Id = Guid.NewGuid(),
				TemplateId = templateId,
				UserId = userId,
				InstructorId = instructorId,
				Parameters = JsonConvert.SerializeObject(parameters),
				Timestamp = DateTime.Now,
				IsPreview = isPreview,
			};
			db.Certificates.Add(certificate);
			await db.SaveChangesAsync();
			return certificate;
		}

		public async Task ChangeTemplateArchiveName(Guid templateId, string newArchiveName)
		{
			var template = FindTemplateById(templateId);
			if (template == null)
				throw new ArgumentException("Invalid templateId", nameof(templateId));

			template.ArchiveName = newArchiveName;
			await db.SaveChangesAsync();
		}

		public async Task ChangeTemplateName(Guid templateId, string name)
		{
			var template = FindTemplateById(templateId);
			if (template == null)
				throw new ArgumentException("Invalid templateId", nameof(templateId));

			template.Name = name;
			await db.SaveChangesAsync();
		}

		public Dictionary<Guid, List<Certificate>> GetCertificates(string courseId, bool includePreviews = false)
		{
			var certificates = db.Certificates
				.Where(c => c.Template.CourseId == courseId && !c.IsDeleted);
			if (!includePreviews)
				certificates = certificates.Where(c => !c.IsPreview);
			return certificates
				.Include(c => c.User)
				.Include(c => c.Instructor)
				.GroupBy(c => c.TemplateId)
				.ToDictionary(g => g.Key, g => g.OrderBy(c => c.Timestamp).ToList());
		}

		public FileInfo GetTemplateArchivePath(CertificateTemplate template)
		{
			return GetTemplateArchivePath(template.ArchiveName);
		}

		private DirectoryInfo GetCertificatesDirectory()
		{
			// var certificatesDirectory = ConfigurationManager.AppSettings["ulearn.certificatesDirectory"];
			var certificatesDirectory = configuration.Certificates.Directory;
			if (string.IsNullOrEmpty(certificatesDirectory))
				certificatesDirectory = Path.Combine(Utils.GetAppPath(), "Certificates");

			var directory = new DirectoryInfo(certificatesDirectory);
			if (!directory.Exists)
				directory.Create();
			return directory;
		}

		public FileInfo GetTemplateArchivePath(string templateArchiveName)
		{
			return GetCertificatesDirectory().GetFile(templateArchiveName + ".zip");
		}

		public DirectoryInfo GetTemplateDirectory(CertificateTemplate template)
		{
			return GetTemplateDirectory(template.ArchiveName);
		}

		public DirectoryInfo GetTemplateDirectory(string templateArchiveName)
		{
			return GetCertificatesDirectory().GetSubdirectory(templateArchiveName);
		}

		public void EnsureCertificateTemplateIsUnpacked(CertificateTemplate template)
		{
			var certificateDirectory = GetTemplateDirectory(template);
			if (!certificateDirectory.Exists)
			{
				log.Info($"Нет директории с распакованным шаблоном сертификата, Id = {template.Id}");

				var certificateArchive = GetTemplateArchivePath(template);
				if (!certificateArchive.Exists)
					throw new Exception("Can\'t find certificate template");

				log.Info($"Распаковываю шаблон сертификата {template.Id}: \"{certificateArchive.FullName}\" в \"{certificateDirectory.FullName}\"");

				using (var zip = ZipFile.Read(certificateArchive.FullName, new ReadOptions { Encoding = Encoding.UTF8 }))
				{
					zip.ExtractAll(certificateDirectory.FullName, ExtractExistingFileAction.OverwriteSilently);
				}
			}
		}

		public IEnumerable<string> GetTemplateParameters(CertificateTemplate template)
		{
			EnsureCertificateTemplateIsUnpacked(template);

			var templateDirectory = GetTemplateDirectory(template);
			var indexFile = templateDirectory.GetFile(TemplateIndexFile);
			if (!indexFile.Exists)
			{
				log.Error($"Не нашёл файла {TemplateIndexFile} в шаблоне \"{template.Name}\" (Id = {template.Id}, {template.ArchiveName})");
				yield break;
			}

			var foundParameters = new HashSet<string>();

			var matches = templateParameterRegex.Matches(File.ReadAllText(indexFile.FullName));
			foreach (Match match in matches)
			{
				var parameter = match.Groups[1].Value;
				if (!foundParameters.Contains(parameter))
				{
					yield return parameter;
					foundParameters.Add(parameter);
				}
			}
		}

		public IEnumerable<string> GetTemplateParametersWithoutBuiltins(CertificateTemplate template)
		{
			return GetTemplateParameters(template).Where(p => !builtInParameters.Contains(p)).Distinct();
		}

		public IEnumerable<string> GetBuiltinTemplateParameters(CertificateTemplate template)
		{
			return GetTemplateParameters(template).Where(p => builtInParameters.Contains(p)).Distinct();
		}

		public string GetTemplateBuiltinParameterForUser(CertificateTemplate template, Course course, ApplicationUser user, ApplicationUser instructor, string parameterName)
		{
			var mockCertificate = new Certificate
			{
				Id = Guid.Empty,
				User = user,
				UserId = user.Id,
				Instructor = instructor,
				InstructorId = instructor.Id,
				Template = template,
				TemplateId = template.Id,
				Timestamp = DateTime.Now,
			};
			return SubstituteBuiltinParameters($"%{parameterName}|raw%", mockCertificate, course, "<адрес сертификата>");
		}

		public string RenderCertificate(Certificate certificate, Course course, string certificateUrl)
		{
			var templateDirectory = GetTemplateDirectory(certificate.Template);
			var indexFile = templateDirectory.GetFile(TemplateIndexFile);
			var content = File.ReadAllText(indexFile.FullName);

			return SubstituteParameters(content, certificate, course, certificateUrl);
		}

		private string SubstituteOneParameter(string content, string parameterName, string parameterValue)
		{
			if (parameterValue == null)
				parameterValue = "";

			content = content.Replace($"%{parameterName}|raw%", parameterValue);

			var htmlEncodedValue = parameterValue.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
			content = content.Replace($"%{parameterName}|in_html%", htmlEncodedValue);
			content = content.Replace($"%{parameterName}%", htmlEncodedValue);

			var quotesEncodedValue = parameterValue.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("'", @"\'");
			content = content.Replace($"%{parameterName}|in_quotes%", quotesEncodedValue);

			return content;
		}

		private string SubstituteParameters(string content, Certificate certificate, Course course, string certificateUrl)
		{
			var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(certificate.Parameters);
			foreach (var kv in parameters)
			{
				content = SubstituteOneParameter(content, kv.Key, kv.Value);
			}

			content = SubstituteBuiltinParameters(content, certificate, course, certificateUrl);

			return content;
		}

		private string SubstituteBuiltinParameters(string content, Certificate certificate, Course course, string certificateUrl)
		{
			content = ReplaceBasicBuiltinParameters(content, certificate, course, certificateUrl);

			/* Replace %score% for total course score */
			var userScore = visitsRepo.GetScoresForSlides(course.Id, certificate.UserId).Sum(p => p.Value);
			content = SubstituteOneParameter(content, "score", userScore.ToString());

			/* Replace %codereviews.*% */
			content = ReplaceCodeReviewsBuiltinParameters(content, certificate, course);
			/* Replace %quizzes.*% */
			content = ReplaceQuizzesBuiltinParameters(content, certificate, course);

			var acceptedSolutionsCount = userSolutionsRepo.GetAllAcceptedSubmissionsByUser(course.Id, course.Slides.Select(s => s.Id), certificate.UserId).Select(s => s.SlideId).Distinct().Count();
			content = SubstituteOneParameter(content, "exercises.accepted", acceptedSolutionsCount.ToString());

			return content;
		}

		private string ReplaceBasicBuiltinParameters(string content, Certificate certificate, Course course, string certificateUrl)
		{
			content = SubstituteOneParameter(content, "user.first_name", certificate.User.FirstName);
			content = SubstituteOneParameter(content, "user.last_name", certificate.User.LastName);
			content = SubstituteOneParameter(content, "user.name", certificate.User.VisibleName);

			content = SubstituteOneParameter(content, "instructor.first_name", certificate.Instructor.FirstName);
			content = SubstituteOneParameter(content, "instructor.last_name", certificate.Instructor.LastName);
			content = SubstituteOneParameter(content, "instructor.name", certificate.Instructor.VisibleName);

			content = SubstituteOneParameter(content, "course.id", course.Id);
			content = SubstituteOneParameter(content, "course.title", course.Title);

			content = SubstituteOneParameter(content, "date", certificate.Timestamp.ToLongDateString());
			content = SubstituteOneParameter(content, "date.day", certificate.Timestamp.Day.ToString());
			content = SubstituteOneParameter(content, "date.month", certificate.Timestamp.Month.ToString("D2"));
			content = SubstituteOneParameter(content, "date.year", certificate.Timestamp.Year.ToString());

			content = SubstituteOneParameter(content, "certificate.id", certificate.Id.ToString());
			content = SubstituteOneParameter(content, "certificate.url", certificateUrl);

			return content;
		}

		private string ReplaceQuizzesBuiltinParameters(string content, Certificate certificate, Course course)
		{
			var passedQuizzesCount = userQuizzesRepo.GetIdOfQuizPassedSlides(course.Id, certificate.UserId).Count;
			var scoredMaximumQuizzesCount = userQuizzesRepo.GetIdOfQuizSlidesScoredMaximum(course.Id, certificate.UserId).Count;

			content = SubstituteOneParameter(content, "quizzes.passed", passedQuizzesCount.ToString());
			content = SubstituteOneParameter(content, "quizzes.passed_maxscore", scoredMaximumQuizzesCount.ToString());
			return content;
		}

		private string ReplaceCodeReviewsBuiltinParameters(string content, Certificate certificate, Course course)
		{
			var codeReviewsCount = slideCheckingsRepo.GetUsersPassedManualExerciseCheckings(course.Id, certificate.UserId).Count();
			var exercisesMaxReviewScores = course.Slides
				.OfType<ExerciseSlide>().
				ToDictionary(s => s.Id, s => s.Exercise.MaxReviewScore);
			var codeReviewsFullCount = slideCheckingsRepo
				.GetUsersPassedManualExerciseCheckings(course.Id, certificate.UserId)
				.Count(s => s.Score == exercisesMaxReviewScores.GetOrDefault(s.SlideId, -1));

			content = SubstituteOneParameter(content, "codereviews.passed", codeReviewsCount.ToString());
			content = SubstituteOneParameter(content, "codereviews.passed_maxscore", codeReviewsFullCount.ToString());
			return content;
		}

		public async Task RemoveTemplate(CertificateTemplate template)
		{
			template.IsDeleted = true;
			await db.SaveChangesAsync();
		}

		public List<Certificate> GetUserCertificates(string userId, bool includePreviews = false)
		{
			var certificates = db.Certificates.Where(c => c.UserId == userId && !c.IsDeleted);
			if (!includePreviews)
				certificates = certificates.Where(c => !c.IsPreview);
			return certificates.ToList();
		}

		public async Task RemoveCertificate(Certificate certificate)
		{
			certificate.IsDeleted = true;
			await db.SaveChangesAsync();
		}
	}
}