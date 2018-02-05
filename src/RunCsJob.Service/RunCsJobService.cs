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
		private readonly RunCsJobProgram runCsJobProgram = new RunCsJobProgram();

		public RunCsJobService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			XmlConfigurator.Configure();
			
			runCsJobProgram.Run(joinAllThreads: false);
		}

		protected override void OnStop()
		{
			runCsJobProgram.Stop();
		}
	}
}