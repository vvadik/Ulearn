using System.Linq;
using System.Threading.Tasks;
using Vostok.Logging.Abstractions;

namespace Database.Repos
{
	public interface ICourseRemover
	{
		Task RemoveCourseWithAllData(string courseId);
	}

	public class CourseRemover: ICourseRemover
	{
		private static ILog log => LogProvider.Get().ForContext(typeof(CourseRemover));

		private UlearnDb db;

		public CourseRemover(UlearnDb db)
		{
			this.db = db;
		}

		public async Task RemoveCourseWithAllData(string courseId)
		{
			log.Info($"Удаляю курс {courseId} со всеми данными");

			using (var transaction = db.Database.BeginTransaction())
			{
				db.AdditionalScores.RemoveRange(db.AdditionalScores.Where(e => e.CourseId == courseId));
				db.CertificateTemplates.RemoveRange(db.CertificateTemplates.Where(e => e.CourseId == courseId));
				db.Comments.RemoveRange(db.Comments.Where(e => e.CourseId == courseId));
				db.CommentsPolicies.RemoveRange(db.CommentsPolicies.Where(e => e.CourseId == courseId));
				db.CourseAccesses.RemoveRange(db.CourseAccesses.Where(e => e.CourseId == courseId));
				db.CourseFiles.RemoveRange(db.CourseFiles.Where(e => e.CourseId == courseId));
				db.CourseGitRepos.RemoveRange(db.CourseGitRepos.Where(e => e.CourseId == courseId));
				db.CourseRoles.RemoveRange(db.CourseRoles.Where(e => e.CourseId == courseId));
				db.CourseVersions.RemoveRange(db.CourseVersions.Where(e => e.CourseId == courseId));
				db.GraderClients.RemoveRange(db.GraderClients.Where(e => e.CourseId == courseId));
				db.Groups.RemoveRange(db.Groups.Where(e => e.CourseId == courseId));
				db.LastVisits.RemoveRange(db.LastVisits.Where(e => e.CourseId == courseId));
				db.LtiRequests.RemoveRange(db.LtiRequests.Where(e => e.CourseId == courseId));
				db.NotificationTransportSettings.RemoveRange(db.NotificationTransportSettings.Where(e => e.CourseId == courseId));
				db.Notifications.RemoveRange(db.Notifications.Where(e => e.CourseId == courseId));
				db.UserQuizSubmissions.RemoveRange(db.UserQuizSubmissions.Where(e => e.CourseId == courseId));
				db.ManualExerciseCheckings.RemoveRange(db.ManualExerciseCheckings.Where(e => e.CourseId == courseId));
				db.AutomaticExerciseCheckings.RemoveRange(db.AutomaticExerciseCheckings.Where(e => e.CourseId == courseId));
				db.ManualQuizCheckings.RemoveRange(db.ManualQuizCheckings.Where(e => e.CourseId == courseId));
				db.AutomaticQuizCheckings.RemoveRange(db.AutomaticQuizCheckings.Where(e => e.CourseId == courseId));
				db.Hints.RemoveRange(db.Hints.Where(e => e.CourseId == courseId));
				db.SlideRates.RemoveRange(db.SlideRates.Where(e => e.CourseId == courseId));
				db.TempCourses.RemoveRange(db.TempCourses.Where(e => e.CourseId == courseId));
				db.TempCourseErrors.RemoveRange(db.TempCourseErrors.Where(e => e.CourseId == courseId));
				db.UnitAppearances.RemoveRange(db.UnitAppearances.Where(e => e.CourseId == courseId));
				db.UserExerciseSubmissions.RemoveRange(db.UserExerciseSubmissions.Where(e => e.CourseId == courseId));
				db.UserFlashcardsUnlocking.RemoveRange(db.UserFlashcardsUnlocking.Where(e => e.CourseId == courseId));
				db.UserFlashcardsVisits.RemoveRange(db.UserFlashcardsVisits.Where(e => e.CourseId == courseId));
				db.UserQuestions.RemoveRange(db.UserQuestions.Where(e => e.CourseId == courseId));
				db.Visits.RemoveRange(db.Visits.Where(e => e.CourseId == courseId));

				await db.SaveChangesAsync();
				await transaction.CommitAsync();
			}

			log.Info($"Курс {courseId} удален со всеми данными");
		}
	}
}