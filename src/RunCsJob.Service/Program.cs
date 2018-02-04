using System.ServiceProcess;

namespace RunCsJob.Service
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main()
		{
			var servicesToRun = new ServiceBase[]
			{
				new RunCsJobService()
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}