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
			var proj = new Project(path, null, null, new ProjectCollection());
			proj.SetProperty("CscToolPath", pathToCompiler);
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