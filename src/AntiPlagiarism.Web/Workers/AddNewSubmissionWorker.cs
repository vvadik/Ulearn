using System;
using System.Collections.Generic;
using System.Threading;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vostok.Logging.Abstractions;

namespace AntiPlagiarism.Web.Workers
{
	public class AddNewSubmissionWorker
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly IOptions<AntiPlagiarismConfiguration> configuration;
		private static ILog log => LogProvider.Get().ForContext(typeof(AddNewSubmissionWorker));

		private readonly List<Thread> threads = new List<Thread>();
		private readonly TimeSpan sleep = TimeSpan.FromSeconds(5);

		public AddNewSubmissionWorker(
			IOptions<AntiPlagiarismConfiguration> configuration,
			IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.configuration = configuration;

			RunHandleNewSubmissionWorkerThreads();
		}
		
		private void RunHandleNewSubmissionWorkerThreads()
		{
			var threadsCount = configuration.Value.AntiPlagiarism.ThreadsCount;
			if (threadsCount < 1)
			{
				log.Error($"Не могу определить количество потоков для запуска из конфигурации: ${threadsCount}. Количество потоков должно быть положительно");
				throw new ArgumentOutOfRangeException(nameof(threadsCount), "Number of threads (antiplagiarism:threadsCount) should be positive");
			}

			log.Info($"Запускаю AddNewSubmissionWorker в {threadsCount} потока(ов)");
			for (var i = 0; i < threadsCount; i++)
			{
				threads.Add(new Thread(WorkerThread)
				{
					Name = $"AddNewSubmissionWorker #{i}",
					IsBackground = true
				});
			}

			threads.ForEach(t => t.Start());
		}

		private void WorkerThread()
		{
			log.Info($"Поток {Thread.CurrentThread.Name} запускается");

			while (true)
			{
				bool newSubmissionHandled = false;
				using (var scope = serviceScopeFactory.CreateScope())
				{
					var newSubmissionHandler = scope.ServiceProvider.GetService<NewSubmissionHandler>();
					try
					{
						newSubmissionHandled = newSubmissionHandler.HandleNewSubmission().Result;
					}
					catch (Exception ex)
					{
						log.Error(ex, "Exception during HandleNewSubmission");
					}
				}
				if(!newSubmissionHandled)
					Thread.Sleep(sleep);
			}
			// ReSharper disable once FunctionNeverReturns
		}
	}
}