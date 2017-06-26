using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using RunCsJob.Api;
using uLearn;

namespace RunCsJob
{
	internal static class RunningResultsExtensions
	{
		public static bool HasErrors(this ImmutableArray<Diagnostic> diagnostics)
		{
			return diagnostics.Count(d => d.DefaultSeverity == DiagnosticSeverity.Error) > 0;
		}

		public static string DumpCompilationOutput(this ImmutableArray<Diagnostic> diagnostics)
		{
			if (diagnostics.Length == 0)
			{
				return "";
			}
			var sb = new StringBuilder();
			var errors = diagnostics.Where(d => d.DefaultSeverity.IsOneOf(DiagnosticSeverity.Error, DiagnosticSeverity.Warning)).ToList();
			foreach (var error in errors)
			{
				sb.Append(error);
			}
			return sb.ToString();
		}

		public static void AddCompilationInfo(this RunningResults results, CompilerResults assembly)
		{
			if (assembly.Errors.Count == 0)
			{
				return;
			}
			var sb = new StringBuilder();
			var errors = assembly.Errors
				.Cast<CompilerError>()
				.ToList();
			foreach (var error in errors)
			{
				sb.Append(string.Format("({2},{3}): {0} {1}: {4}\n", error.IsWarning ? "warning" : "error", error.ErrorNumber,
					error.Line, error.Column,
					error.ErrorText));
			}
			if (assembly.Errors.HasErrors)
				results.Verdict = Verdict.CompilationError;
			results.CompilationOutput = sb.ToString();
		}
	}
}