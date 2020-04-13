using System;
using System.Collections.Generic;
using System.Threading;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AntiPlagiarism.Web.Database
{
	public class AddNewSubmissionWorker
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly IOptions<AntiPlagiarismConfiguration> configuration;
		private readonly ILogger logger;
		
		private readonly List<Thread> threads = new List<Thread>();
		private readonly TimeSpan sleep = TimeSpan.FromSeconds(5);

		public AddNewSubmissionWorker(ILogger logger,
			IOptions<AntiPlagiarismConfiguration> configuration,
			IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.logger = logger;
			this.configuration = configuration;

			RunHandleNewSubmissionWorkerThreads();
		}
		
		private void RunHandleNewSubmissionWorkerThreads()
		{
			var threadsCount = configuration.Value.AntiPlagiarism.ThreadsCount;
			if (threadsCount < 1)
			{
				logger.Error($"Не могу определить количество потоков для запуска из конфигурации: ${threadsCount}. Количество потоков должно быть положительно");
				throw new ArgumentOutOfRangeException(nameof(threadsCount), "Number of threads (antiplagiarism:threadsCount) should be positive");
			}

			logger.Information($"Запускаю {threadsCount} потока(ов)");
			for (var i = 0; i < threadsCount; i++)
			{
				threads.Add(new Thread(WorkerThread)
				{
					Name = $"Worker #{i}",
					IsBackground = true
				});
			}

			threads.ForEach(t => t.Start());
		}

		private void WorkerThread()
		{
			logger.Information($"Поток {Thread.CurrentThread.Name} запускается");
			
			while (true)
			{
				bool newSubmissionHandled;
				using (var scope = serviceScopeFactory.CreateScope())
				{
					var newSubmissionHandler = scope.ServiceProvider.GetService<NewSubmissionHandler>();
					newSubmissionHandled = newSubmissionHandler.HandleNewSubmission().Result;
				}
				if(!newSubmissionHandled)
					Thread.Sleep(sleep);
			}
			// ReSharper disable once FunctionNeverReturns
		}
	}
}