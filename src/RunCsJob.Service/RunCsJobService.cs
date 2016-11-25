using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RunCsJob.Service
{
	public partial class RunCsJobService : ServiceBase
	{
		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
		private Thread thread;

		public RunCsJobService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			thread = new Thread(WorkerThread)
			{
				Name = "RunCsJob Worker Thread",
				IsBackground = true
			};
			thread.Start();
		}

		protected override void OnStop()
		{
			shutdownEvent.Set();

			if (!thread.Join(10000))
			{ 
				thread.Abort();
			}
		}

		private void WorkerThread()
		{
			var pathToCompilers = Path.Combine(new DirectoryInfo(".").FullName, "Microsoft.Net.Compilers.1.3.2");
			new RunCsJobProgram(shutdownEvent).Run(pathToCompilers);
		}
	}
}
