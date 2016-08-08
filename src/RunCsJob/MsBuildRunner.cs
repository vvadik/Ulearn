using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using RunCsJob.Api;

namespace RunCsJob
{
	public static class MsBuildRunner
	{
		public static MSBuildResult BuildProject(string projectFileName, DirectoryInfo dir)
		{
			var result = new MSBuildResult();
			var path = Path.Combine(dir.FullName, projectFileName);
			var proj = new Project(path);
			using (var stringWriter = new StringWriter())
			{
				var logger = new ConsoleLogger(LoggerVerbosity.Minimal, stringWriter.Write, color => { }, () => { });
				result.Success = proj.Build(logger);
				if (result.Success)
					result.PathToExe = Path.Combine(proj.DirectoryPath, proj.GetPropertyValue("OutputPath"), proj.GetPropertyValue("AssemblyName") + ".exe");
				else
					result.ErrorMessage = stringWriter.ToString();
				return result;
			}
		}
	}
}