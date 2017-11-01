using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using RunCsJob.Api;

namespace RunCsJob
{
	public static class AssemblyCreator
	{
		public static CompileResult CreateAssemblyWithRoslyn(FileRunnerSubmission submission, string workingDirectory)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(submission.Code);
			var assemblyName = submission.Id;

			var compilation = CSharpCompilation.Create(
				assemblyName, new[] { syntaxTree },
				new MetadataReference[]
				{
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // mscorlib
					MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location), // System
					MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Core
					MetadataReference.CreateFromFile(typeof(Point).Assembly.Location), // System.Drawing,
					MetadataReference.CreateFromFile(typeof(ValueType).Assembly.Location), // System.Runtime
				}, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
			
			var assemblyFilename = Path.Combine(workingDirectory, assemblyName + ".exe");
			return new CompileResult(compilation.Emit(assemblyFilename), assemblyFilename);
		}
	}

	public class CompileResult
	{
		public readonly EmitResult EmitResult;
		public readonly string PathToAssembly;

		public CompileResult(EmitResult emitResult, string pathToAssembly)
		{
			EmitResult = emitResult;
			PathToAssembly = pathToAssembly;
		}
	}
}