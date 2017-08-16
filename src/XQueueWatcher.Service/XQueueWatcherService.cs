using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using XQueueWatcher;

namespace XQueueWatcher.Service
{
	public partial class XQueueWatcherService : ServiceBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XQueueWatcherService));

		private XQueueWatcher.Program program;
		private Thread thread;

		public XQueueWatcherService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			program = new XQueueWatcher.Program();
			log.Info("Запускаю сервис проверки решений из внешних XQueue");
			thread = new Thread(() => program.StartXQueueWatchers())
			{
				Name = "XQueueWatcher service thread",
				IsBackground = true,
			};
			thread.Start();
		}

		protected override void OnStop()
		{
			log.Info("Пробую остановить сервис проверки решений из внешних XQueue");

			program.CancellationTokenSource.Cancel();
			if (!thread.Join(10000))
			{
				log.Info($"Вызываю Abort() для потока {thread.Name}");
				thread.Abort();
			}

			log.Info("Сервис проверки решений из внешних XQueue остановлен");
		}
	}
}
