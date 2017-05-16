using System.ServiceProcess;
using System.Threading;
using log4net;
using log4net.Config;

namespace Notifications.Service
{
	public partial class NotificationsService : ServiceBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(NotificationsService));

		private Notifications.Program program;
		private Thread thread;

		public NotificationsService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			XmlConfigurator.Configure();

			program = new Notifications.Program();
			log.Info("Запускаю сервис уведомлений");
			thread = new Thread(() => program.MainLoop().Wait())
			{
				Name = "Notifications service thread",
				IsBackground = true,
			};
			thread.Start();
		}

		protected override void OnStop()
		{
			log.Info("Пробую остановить сервис уведомлений");

			program.Stopped = true;
			if (!thread.Join(10000))
			{
				log.Info($"Вызываю Abort() для потока {thread.Name}");
				thread.Abort();
			}

			log.Info("Сервис уведомлений остановлен");
		}
	}
}
