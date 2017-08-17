using System.ServiceProcess;
using System.Threading;
using log4net;

namespace XQueueWatcher.Service
{
	public partial class XQueueWatcherService : ServiceBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XQueueWatcherService));

		private XQueueWatcher.Program program;
		private Thread thread;
		private CancellationTokenSource cancellationTokenSource;

		public XQueueWatcherService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			program = new XQueueWatcher.Program();
			log.Info("Запускаю сервис проверки решений из внешних XQueue");
			cancellationTokenSource = new CancellationTokenSource();
			thread = new Thread(() => program.StartXQueueWatchers(cancellationTokenSource.Token))
			{
				Name = "XQueueWatcher service thread",
				IsBackground = true,
			};
			thread.Start();
		}

		protected override void OnStop()
		{
			log.Info("Пробую остановить сервис проверки решений из внешних XQueue");

			cancellationTokenSource.Cancel();
			if (!thread.Join(10000))
			{
				log.Info($"Вызываю Abort() для потока {thread.Name}");
				thread.Abort();
			}

			log.Info("Сервис проверки решений из внешних XQueue остановлен");
		}
	}
}
