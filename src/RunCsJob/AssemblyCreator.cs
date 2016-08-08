using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using RunCsJob.Api;

namespace RunCsJob
{
	public class AssemblyCreator
	{
		private static readonly string[] UsesAssemblies =
		{
			"System.dll",
			"System.Core.dll",
			"System.Linq.dll",
			"System.Drawing.dll",
			"mscorlib.dll"
		};

		public static CompilerResults CreateAssembly(FileRunnerSubmition submission)
		{
			var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
			var compilerParameters = new CompilerParameters(UsesAssemblies)
			{
				GenerateExecutable = true,
				IncludeDebugInformation = true,
				WarningLevel = 4,
			};

			var assembly = provider.CompileAssemblyFromSource(compilerParameters, submission.Code);

			return assembly;
		}
	}
}