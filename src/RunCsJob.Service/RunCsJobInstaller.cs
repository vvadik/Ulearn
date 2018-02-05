using System.ComponentModel;

namespace RunCsJob.Service
{
	[RunInstaller(true)]
	public partial class RunCsJobInstaller : System.Configuration.Install.Installer
	{
		public RunCsJobInstaller()
		{
			InitializeComponent();
		}
	}
}
