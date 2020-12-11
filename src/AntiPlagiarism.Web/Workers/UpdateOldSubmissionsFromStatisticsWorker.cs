using System;
using System.Threading;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vostok.Logging.Abstractions;

namespace AntiPlagiarism.Web.Workers
{
	public class UpdateOldSubmissionsFromStatisticsWorker
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly AntiPlagiarismConfiguration configuration;
		private readonly ILog log = LogProvider.Get().ForContext(typeof(UpdateOldSubmissionsFromStatisticsWorker));
// ReSharper disable once NotAccessedField.Local
		private readonly Timer timer;

		public UpdateOldSubmissionsFromStatisticsWorker(
			IOptions<AntiPlagiarismConfiguration> configuration,
			IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.configuration = configuration.Value;
			timer = CreateWorkerTimer();
		}

		private Timer CreateWorkerTimer()
		{
			var startTime = configuration.AntiPlagiarism.Actions.UpdateOldSubmissionsFromStatistics.StartTime;
			if (startTime == null)
				return null;
			var now = DateTime.Now;
			var startDateTime = new DateTime(now.Year, now.Month, now.Day, startTime.Value.Hour, startTime.Value.Minute, startTime.Value.Second);
			if (startDateTime < now)
				startDateTime = startDateTime.Add(TimeSpan.FromDays(1));
			var diffFromNowToStart = startDateTime - DateTime.Now;
			return new Timer(ThreadFunc, null, diffFromNowToStart, TimeSpan.FromDays(1));
		}

		private void ThreadFunc(object stateInfo)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var snippetsRepo = scope.ServiceProvider.GetService<ISnippetsRepo>();
				try
				{
					var now = DateTime.Now;
					var submissionInfluenceLimitInMonths = configuration.AntiPlagiarism.SubmissionInfluenceLimitInMonths;
					var border = snippetsRepo.GetOldSubmissionsInfluenceBorderAsync().Result;
					var from = border?.Date ?? new DateTime(2000, 1, 1);
					var to = now.AddMonths(-submissionInfluenceLimitInMonths);
					snippetsRepo.UpdateOldSnippetsStatisticsAsync(from, to).Wait();
					snippetsRepo.SetOldSubmissionsInfluenceBorderAsync(to).Wait();
				}
				catch (Exception ex)
				{
					log.Error(ex, "Exception during UpdateOldSubmissionsFromStatistics");
				}
			}
		}
	}
}