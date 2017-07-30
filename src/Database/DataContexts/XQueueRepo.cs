using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;
using uLearn;

namespace Database.DataContexts
{
	public class XQueueRepo
	{
		private readonly ULearnDb db;
		private readonly ULearnUserManager userManager;
		private readonly UserSolutionsRepo userSolutionsRepo;

		public XQueueRepo(ULearnDb db, CourseManager courseManager)
		{
			this.db = db;
			userManager = new ULearnUserManager(db);
			userSolutionsRepo = new UserSolutionsRepo(db, courseManager);
		}

		public async Task AddXQueueWatcher(string name, string baseUrl, string queueName, string username, string password)
		{
			var user = new ApplicationUser { UserName = $"__xqueue_watcher_{new Guid().GetNormalizedGuid()}__" };
			var userPassword = uLearn.Helpers.StringUtils.GenerateSecureAlphanumericString(10);
			await userManager.CreateAsync(user, userPassword);
			var watcher = new XQueueWatcher
			{
				Name = name,
				BaseUrl = baseUrl,
				QueueName = queueName,
				UserName = username,
				Password = password,
				IsEnabled = true,
				User = user,
			};
			db.XQueueWatchers.Add(watcher);

			await db.SaveChangesAsync();
		}

		public List<XQueueWatcher> GetXQueueWatchers()
		{
			return db.XQueueWatchers.Where(w => w.IsEnabled).ToList();
		}

		[CanBeNull]
		public XQueueExerciseSubmission FindXQueueSubmission(UserExerciseSubmission submission)
		{
			return db.XQueueExerciseSubmissions.FirstOrDefault(s => s.SubmissionId == submission.Id);
		}

		public async Task AddXQueueSubmission(XQueueWatcher watcher, string xQueueHeader, string courseId, Guid slideId, string code)
		{
			var submission = await userSolutionsRepo.AddUserExerciseSubmission(
				courseId, slideId, code, null, null, watcher.UserId,
				"uLearn", $"XQueue watcher {watcher.Name}"
			).ConfigureAwait(false);
			db.XQueueExerciseSubmissions.Add(new XQueueExerciseSubmission
			{
				SubmissionId = submission.Id,
				WatcherId = watcher.Id,
				XQueueHeader = xQueueHeader,
			});

			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}