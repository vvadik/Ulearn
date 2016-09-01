using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;
using RunCsJob.Api;

namespace RunCsJob
{
	public static class AssemblyCreator
	{
		private static readonly string[] UsesAssemblies =
		{
			"System.dll",
			"System.Core.dll",
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

		public static IEnumerable<int> x = Enumerable.Range(1, 1);

		public static CompileResult CreateAssemblyWithRoslyn(FileRunnerSubmition submission)
		{
			IEnumerable<int> x = null;
			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(submission.Code);
			var compilation = CSharpCompilation.Create(submission.Id, new[] { syntaxTree },
				new MetadataReference[]
				{
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // mscorlib
					MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location), // System
					MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Core
					MetadataReference.CreateFromFile(typeof(Point).Assembly.Location), //System.Drawing
				}, new CSharpCompilationOptions(OutputKind.ConsoleApplication));

			var assemblyFilename = submission.Id + ".exe";
			return new CompileResult(compilation.Emit(assemblyFilename), assemblyFilename);
		}

	}
	public class CompileResult
	{
		public CompileResult(EmitResult emitResult, string pathToAssembly)
		{
			EmitResult = emitResult;
			PathToAssembly = pathToAssembly;
		}

		public EmitResult EmitResult;
		public string PathToAssembly;

	}

}