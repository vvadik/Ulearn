using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using log4net;
using log4net.Config;

namespace RunCsJob.Service
{
	public partial class RunCsJobService : ServiceBase
	{
		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
		private readonly List<Thread> threads = new List<Thread>();
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCsJobService));

		public RunCsJobService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			XmlConfigurator.Configure();

			var threadsCount = int.Parse(ConfigurationManager.AppSettings["threadsCount"] ?? "1");
			if (threadsCount < 1)
				throw new ArgumentOutOfRangeException(nameof(threadsCount), "Number of threads (appSettings/threadsCount) should be positive");
			log.Info($"Start {threadsCount} threads");
			for (var i = 0; i < threadsCount; i++)
			{
				threads.Add(new Thread(WorkerThread)
				{
					Name = $"RunCsJob Worker Thread #{i}",
					IsBackground = true
				});
			}
			threads.ForEach(t => t.Start());
		}

		protected override void OnStop()
		{
			shutdownEvent.Set();
			log.Info("Received signal for stopping");

			foreach (var thread in threads)
			{
				log.Info($"Try to stop {thread.Name}");
				if (!thread.Join(10000))
				{
					log.Info($"Abort {thread.Name}");
					thread.Abort();
				}
			}
		}

		private void WorkerThread()
		{
			log.Info($"{Thread.CurrentThread.Name} is starting");
			var pathToCompilers = Path.Combine(new DirectoryInfo(".").FullName, "Microsoft.Net.Compilers.1.3.2");
			new RunCsJobProgram(shutdownEvent).Run(pathToCompilers);
		}
	}
}
