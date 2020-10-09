using System;
using System.IO;
using Microsoft.Build.Utilities;

namespace Ulearn.Core.Helpers
{
	public static class MsBuildLocationHelper
	{
		private static string pathToMsBuild;

		public static void InitPathToMsBuild()
		{
			if (pathToMsBuild != null)
				return;
			const string version = "Current";
			var path = ToolLocationHelper.GetPathToBuildTools(version);
			var assembly = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(ProjModifier)).Location);
			if (path == assembly)
				Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe");
			pathToMsBuild = ToolLocationHelper.GetPathToBuildTools(version);
		}
	}
}