using System;
using System.Diagnostics;
using System.IO;
using CommandLine;

namespace uLearn.CourseTool.CmdLineOptions
{
	[Verb("build-containers", HelpText = "build docker containers for checking solutions of exercises in sanbox")]
	public class BuildDockerContainerOptions : AbstractOptions
	{
		public override void DoExecute()
		{
			var pathToBuildBat = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BuildDockerContainerOptions)).Location), "sandboxes/build.bat");
			var process = new Process
			{
				StartInfo =
				{
					Arguments = "",
					FileName = pathToBuildBat,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
					UseShellExecute = false,
					WorkingDirectory = Path.GetDirectoryName(pathToBuildBat)
				}
			};
			process.Start();
			process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
			process.BeginOutputReadLine();

			process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
			process.BeginErrorReadLine();
			process.WaitForExit();
			process.Close();
		}
	}
}