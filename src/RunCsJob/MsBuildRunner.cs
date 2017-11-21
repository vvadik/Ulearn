using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace RunCsJob
{
	public class MsBuildSettings
	{
		private const string CompilersFolderName = "Microsoft.Net.Compilers.2.4.0";
		private const string WellKnownLibsFolderName = "WellKnownLibs";

		public MsBuildSettings()
		{
			BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			CompilerDirectory = new DirectoryInfo(Path.Combine(BaseDirectory, CompilersFolderName));
			WellKnownLibsDirectory = new DirectoryInfo(Path.Combine(BaseDirectory, WellKnownLibsFolderName));
		}

		public readonly string BaseDirectory;
		public DirectoryInfo CompilerDirectory;
		public DirectoryInfo WellKnownLibsDirectory;
		public readonly string MsBuildToolsVersion = "14.0";
	}

	public static class MsBuildRunner
	{
		private static readonly string[] obligatoryLibs = {"System.Runtime", "System.Reflection"};

		public static MSbuildResult BuildProject(MsBuildSettings settings, string projectFileName, DirectoryInfo dir)
		{
			var result = new MSbuildResult();
			var path = Path.Combine(dir.FullName, projectFileName);
			var project = new Project(path, null, settings.MsBuildToolsVersion, new ProjectCollection());
			project.SetProperty("CscToolPath", settings.CompilerDirectory.FullName);

			foreach (var libName in obligatoryLibs)
			{
				if (!project.HasReference(libName))
					project.AddReference(libName);
			}
			project.ReevaluateIfNecessary();

			var includes = new HashSet<string>(
				project.AllEvaluatedItems
					.Where(i => i.ItemType == "None" || i.ItemType == "Content")
					.Select(i => Path.GetFileName(i.EvaluatedInclude.ToLowerInvariant())));

			foreach (var dll in settings.WellKnownLibsDirectory.GetFiles("*.dll"))
				if (!includes.Contains(dll.Name.ToLowerInvariant()))
					project.AddItem("None", dll.FullName);

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