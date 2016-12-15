using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace RunCsJob
{
	public static class MsBuildRunner
	{
		public static MSbuildResult BuildProject(string pathToCompiler,string projectFileName, DirectoryInfo dir)
		{
			var result = new MSbuildResult();
			var path = Path.Combine(dir.FullName, projectFileName);
			var project = new Project(path, null, null, new ProjectCollection());
			project.SetProperty("CscToolPath", pathToCompiler);
			project.Save();
			using (var stringWriter = new StringWriter())
			{
				var logger = new ConsoleLogger(LoggerVerbosity.Minimal, stringWriter.Write, color => { }, () => { });
				result.Success = SyncBuild(project, logger);
				if (result.Success)
					result.PathToExe = Path.Combine(project.DirectoryPath,
													project.GetPropertyValue("OutputPath"),
													project.GetPropertyValue("AssemblyName") + ".exe");
				else
					result.ErrorMessage = stringWriter.ToString();
				return result;
			}
		}

		private static readonly object buildLock = new object();

		private static bool SyncBuild(Project project, ILogger logger)
		{
			lock (buildLock)
				return project.Build(logger);
		}
	}
}