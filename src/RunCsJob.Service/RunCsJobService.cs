using System;
using System.Collections.Generic;
using System.Configuration;
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

			var threadsCount = int.Parse(ConfigurationManager.AppSettings["ulearn.runcsjob.threadsCount"] ?? "1");
			if (threadsCount < 1)
			{
				log.Error($"Не могу определить количество потоков для запуска из конфигурации: ${threadsCount}. Количество потоков должно быть положительно");
				throw new ArgumentOutOfRangeException(nameof(threadsCount), "Number of threads (appSettings/threadsCount) should be positive");
			}
			log.Info($"Запускаю {threadsCount} потока(ов)");
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
			log.Info("Получен сигнал остановки");

			foreach (var thread in threads)
			{
				log.Info($"Пробую остановить поток {thread.Name}");
				if (!thread.Join(10000))
				{
					log.Info($"Вызываю Abort() для потока {thread.Name}");
					thread.Abort();
				}
			}
		}

		private void WorkerThread()
		{
			log.Info($"Поток {Thread.CurrentThread.Name} запускается");
			new RunCsJobProgram(shutdownEvent).Run();
		}
	}
}
