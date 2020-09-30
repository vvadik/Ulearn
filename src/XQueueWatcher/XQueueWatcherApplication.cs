using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Common.Extensions;
using Ulearn.Core.Metrics;
using Vostok.Hosting.Abstractions;
using XQueue;
using XQueue.Models;

namespace XQueueWatcher
{
	public class XQueueWatcherApplication : BaseApplication
	{
		private static readonly TimeSpan pauseBetweenRequests = TimeSpan.FromSeconds(1);
		private static readonly ServiceKeepAliver keepAliver = new ServiceKeepAliver(configuration.GraphiteServiceName);
		private static readonly Dictionary<int, XQueueClient> clientsCache = new Dictionary<int, XQueueClient>();

		public override async Task InitializeAsync(IVostokHostingEnvironment environment)
		{
			await base.InitializeAsync(environment);
		}

		public override async Task RunAsync(IVostokHostingEnvironment environment)
		{
			await StartXQueueWatchers(new CancellationToken());
		}

		public async Task StartXQueueWatchers(CancellationToken cancellationToken)
		{
			var keepAliveInterval = TimeSpan.FromSeconds(configuration.KeepAliveInterval ?? 30);

			while (true)
			{
				var xQueueRepo = serviceProvider.GetService<IXQueueRepo>();
				var dbWatchers = await xQueueRepo.GetXQueueWatchers();

				var tasks = dbWatchers.Select(SafeGetAndProcessSubmissionFromXQueue);

				Task.WaitAll(tasks.ToArray(), cancellationToken);
				if (cancellationToken.IsCancellationRequested)
					break;

				Task.Delay(pauseBetweenRequests, cancellationToken).Wait(cancellationToken);
				keepAliver.Ping(keepAliveInterval);
			}
		}

		private async Task SafeGetAndProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher)
		{
			try
			{
				await GetAndProcessSubmissionFromXQueue(watcher);
			}
			catch (Exception e)
			{
				logger.Error(e, "GetAndProcessSubmissionFromXQueue error");
			}
		}

		private async Task GetAndProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher)
		{
			if (!clientsCache.TryGetValue(watcher.Id, out var client))
			{
				client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);

				if (!await client.Login())
				{
					logger.Error($"Can\'t login to xqueue {watcher.QueueName} ({watcher.BaseUrl}, user {watcher.UserName})");
					return;
				}

				clientsCache[watcher.Id] = client;
			}

			var submission = await client.GetSubmission(watcher.QueueName);
			if (submission == null)
				return;

			logger.Information($"Got new submission in xqueue {watcher.QueueName}: {submission.JsonSerialize()}");

			try
			{
				await ProcessSubmissionFromXQueue(watcher, submission);
			}
			catch (Exception e)
			{
				logger.Error($"Error occurred while processing submission from xqueue: {e.Message}", e);
				await client.PutResult(new XQueueResult
				{
					header = submission.header,
					Body = new XQueueResultBody
					{
						Score = 0,
						IsCorrect = false,
						Message = "Sorry, we can't check your code now. Please retry or contact us at support@ulearn.me",
					}
				});
			}
		}

		private async Task ProcessSubmissionFromXQueue(Database.Models.XQueueWatcher watcher, XQueueSubmission submission)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var xQueueRepo = scope.ServiceProvider.GetService<IXQueueRepo>();
				GraderPayload graderPayload;
				try
				{
					graderPayload = submission.Body.GraderPayload.DeserializeJson<GraderPayload>();
				}
				catch (Exception e)
				{
					logger.Error($"Can't parse grader payload: {e.Message}");
					logger.Information($"Payload is: {submission.Body.GraderPayload}");
					return;
				}

				logger.Information($"Add new xqueue submission for course {graderPayload.CourseId}, slide {graderPayload.SlideId}");
				await xQueueRepo.AddXQueueSubmission(
					watcher,
					submission.Header.JsonSerialize(),
					graderPayload.CourseId,
					graderPayload.SlideId,
					submission.Body.StudentResponse
				);
			}
		}
	}

	[DataContract]
	public class GraderPayload
	{
		[DataMember(Name = "course_id")]
		public string CourseId;

		[DataMember(Name = "slide_id")]
		public Guid SlideId;
	}
}