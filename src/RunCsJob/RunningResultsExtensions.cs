using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using Microsoft.CodeAnalysis;
using RunCsJob.Api;
using uLearn;

namespace RunCsJob
{
	internal static class RunningResultsExtensions
	{
		public static void AddCompilationInfo(this RunningResults results, ImmutableArray<Diagnostic> diagnostics)
		{
			if (diagnostics.Length == 0)
			{
				return;
			}
			var sb = new StringBuilder();
			var errors = diagnostics.Where(d => d.DefaultSeverity.IsOneOf(DiagnosticSeverity.Error, DiagnosticSeverity.Warning)).ToList();
			foreach (var error in errors)
			{
				sb.Append(error);
			}
			if (errors.Any(e => e.DefaultSeverity == DiagnosticSeverity.Error))
				results.Verdict = Verdict.CompilationError;
			results.CompilationOutput = sb.ToString();
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

		public static void HandleException(this RunningResults results, Exception ex)
		{
			HandleException(ref results, (dynamic)ex);
		}

		public static bool IsCompilationError(this RunningResults results)
		{
			return results.Verdict == Verdict.CompilationError;
		}

		private static void HandleException(ref RunningResults results, Exception ex)
		{
			results.Verdict = Verdict.SandboxError;
			results.Error = ex.ToString();
		}

		private static void HandleException(ref RunningResults results, TargetInvocationException ex)
		{
			HandleInnerException(ref results, (dynamic)ex.InnerException);
		}

		private static void HandleInnerException(ref RunningResults results, SecurityException ex)
		{
			results.Verdict = Verdict.SecurityException;
			results.Error = ex.ToString();
		}

		private static void HandleInnerException(ref RunningResults results, MemberAccessException ex)
		{
			results.Verdict = Verdict.SecurityException;
			results.Error = ex.ToString();
		}

		private static void HandleInnerException(ref RunningResults results, TypeInitializationException ex)
		{
			results.Verdict = Verdict.SecurityException;
			results.Error = ex.ToString();
		}

		private static void HandleInnerException(ref RunningResults results, Exception ex)
		{
			results.Verdict = Verdict.RuntimeError;
			results.Error = ex.ToString();
		}

	}
}