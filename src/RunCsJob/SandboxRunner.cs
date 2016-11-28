using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RunCsJob.Api;
using uLearn;

namespace RunCsJob
{
	public class SandboxRunner
	{
		private readonly RunnerSubmission submission;

		private const int timeLimitInSeconds = 10;
		private static readonly TimeSpan timeLimit = TimeSpan.FromSeconds(timeLimitInSeconds);
		private static readonly TimeSpan idleTimeLimit = TimeSpan.FromSeconds(2 * timeLimitInSeconds);

		private const int memoryLimit = 64 * 1024 * 1024;
		private const int outputLimit = 10 * 1024 * 1024;
		private bool hasTimeLimit;
		private bool hasMemoryLimit;
		private bool hasOutputLimit;

		private readonly RunningResults result = new RunningResults();

		public static RunningResults Run(string pathToCompiler, RunnerSubmission submission)
		{
			try
			{
				if (submission is ProjRunnerSubmission)
					return new SandboxRunner(submission).RunMsBuild(pathToCompiler);
				return new SandboxRunner(submission).RunCsc60();
			}
			catch (Exception ex)
			{
				return new RunningResults
				{
					Id = submission.Id,
					Verdict = Verdict.SandboxError,
					Error = ex.ToString()
				};
			}
		}

		public SandboxRunner(RunnerSubmission submission)
		{
			this.submission = submission;
			result.Id = submission.Id;
		}

		private RunningResults RunMsBuild(string pathToCompiler)
		{
			var projSubmition = (ProjRunnerSubmission)submission;
			var dir = Directory.CreateDirectory(Path.Combine(".", "submitions", submission.Id));
			try
			{
				try
				{
					Utils.UnpackZip(projSubmition.ZipFileData, dir.FullName);
				}
				catch (Exception ex)
				{
					return new RunningResults
					{
						Id = submission.Id,
						Verdict = Verdict.SandboxError,
						Error = ex.ToString()
					};
				}

				var builderResult = MsBuildRunner.BuildProject(pathToCompiler, projSubmition.ProjectFileName, dir);
				result.Verdict = Verdict.Ok;

				if (!builderResult.Success)
				{
					result.Verdict = Verdict.CompilationError;
					result.CompilationOutput = builderResult.ToString();
					return result;
				}
				RunSandboxer($"\"{builderResult.PathToExe}\" {submission.Id}");
				return result;
			}
			finally
			{
				//SafeRemoveDirectory(dir.FullName);
			}
		}

		public RunningResults RunCsc()
		{
			var assembly = AssemblyCreator.CreateAssembly((FileRunnerSubmission)submission);

			result.Verdict = Verdict.Ok;
			result.AddCompilationInfo(assembly);

			if (result.IsCompilationError())
			{
				SafeRemoveFile(assembly.PathToAssembly);
				return result;
			}

			if (!submission.NeedRun)
			{
				SafeRemoveFile(assembly.PathToAssembly);
				return result;
			}
			RunSandboxer($"\"{Path.GetFullPath(assembly.PathToAssembly)}\" {submission.Id}");

			SafeRemoveFile(assembly.PathToAssembly);
			return result;
		}

		public RunningResults RunCsc60()
		{
			var res = AssemblyCreator.CreateAssemblyWithRoslyn((FileRunnerSubmission)submission);

			result.Verdict = Verdict.Ok;
			result.AddCompilationInfo(res.EmitResult.Diagnostics);

			if (result.IsCompilationError())
			{
				SafeRemoveFile(res.PathToAssembly);
				return result;
			}

			if (!submission.NeedRun)
			{
				SafeRemoveFile(res.PathToAssembly);
				return result;
			}
			RunSandboxer($"\"{Path.GetFullPath(res.PathToAssembly)}\" {submission.Id}");

			SafeRemoveFile(res.PathToAssembly);
			return result;
		}

		private static void SafeRemoveDirectory(string path)
		{
			try
			{
				Directory.Delete(path, true);
			}
			catch
			{
			}
		}

		private static void SafeRemoveFile(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
		}

		private void RunSandboxer(string args)
		{
			var startInfo = new ProcessStartInfo(
				"CsSandboxer.exe", args)
			{
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			var sandboxer = Process.Start(startInfo);

			if (sandboxer == null)
			{
				result.Verdict = Verdict.SandboxError;
				result.Error = "Can't start proces";
				return;
			}

			var stderrReader = new AsyncReader(sandboxer.StandardError, outputLimit + 1);

			var readyState = sandboxer.StandardOutput.ReadLineAsync();
			if (!readyState.Wait(timeLimit) || readyState.Result != "Ready")
			{
				if (!sandboxer.HasExited)
				{
					sandboxer.Kill();
					result.Verdict = Verdict.SandboxError;
					result.Error = "Sandbox does not respond";
					return;
				}
				if (sandboxer.ExitCode != 0)
				{
					HandleNonZeroExitCode(stderrReader.GetData(), sandboxer.ExitCode);
					return;
				}
				result.Verdict = Verdict.SandboxError;
				result.Error = "Sandbox exit before respond";
				return;
			}

			sandboxer.Refresh();
			var startUsedMemory = sandboxer.WorkingSet64;
			var startUsedTime = sandboxer.TotalProcessorTime;
			var startTime = DateTime.Now;

			sandboxer.StandardInput.WriteLine("Run");
			sandboxer.StandardInput.WriteLineAsync(submission.Input);

			var stdoutReader = new AsyncReader(sandboxer.StandardOutput, outputLimit + 1);
			while (!sandboxer.HasExited
					&& !IsTimeLimitExpected(sandboxer, startTime, startUsedTime)
					&& !IsMemoryLimitExpected(sandboxer, startUsedMemory)
					&& !IsOutputLimit(stdoutReader)
					&& !IsOutputLimit(stderrReader))
			{
			}

			if (!sandboxer.HasExited)
				sandboxer.Kill();

			if (hasOutputLimit)
			{
				result.Verdict = Verdict.OutputLimit;
				return;
			}

			if (hasTimeLimit)
			{
				result.Verdict = Verdict.TimeLimit;
				return;
			}

			if (hasMemoryLimit)
			{
				result.Verdict = Verdict.MemoryLimit;
				return;
			}

			sandboxer.WaitForExit();
			if (sandboxer.ExitCode != 0)
			{
				HandleNonZeroExitCode(stderrReader.GetData(), sandboxer.ExitCode);
				return;
			}

			result.Output = stdoutReader.GetData();
			result.Error = stderrReader.GetData();
		}

		private void HandleNonZeroExitCode(string error, int exitCode)
		{
			var obj = FindSerializedException(error);

			if (obj != null)
				result.HandleException(obj);
			else
				HandleNtStatus(exitCode, error);
		}

		private void HandleNtStatus(int exitCode, string error)
		{
			switch ((uint)exitCode)
			{
				case 0xC00000FD:
					result.Verdict = Verdict.RuntimeError;
					result.Error = "Stack overflow exception.";
					break;
				default:
					result.Verdict = Verdict.SandboxError;
					result.Error = string.IsNullOrWhiteSpace(error) ? "Non-zero exit code" : error;
					result.Error += $"\nExit code: 0x{exitCode:X8}";
					break;
			}
		}

		private bool IsOutputLimit(AsyncReader reader)
		{
			return hasOutputLimit = hasOutputLimit
									|| (reader.ReadedLength > outputLimit);
		}

		private bool IsMemoryLimitExpected(Process sandboxer, long startUsedMemory)
		{
			sandboxer.Refresh();
			long mem;
			try
			{
				mem = sandboxer.PeakWorkingSet64;
			}
			catch
			{
				return hasMemoryLimit;
			}

			return hasMemoryLimit = hasMemoryLimit
									|| startUsedMemory + memoryLimit < mem;
		}

		private bool IsTimeLimitExpected(Process sandboxer, DateTime startTime, TimeSpan startUsedTime)
		{
			return hasTimeLimit = hasTimeLimit
								|| timeLimit.Add(startUsedTime).CompareTo(sandboxer.TotalProcessorTime) < 0
								|| startTime.Add(idleTimeLimit).CompareTo(DateTime.Now) < 0;
		}

		private static Exception FindSerializedException(string str)
		{
			if (!str.EndsWith("}"))
				return null;

			var pos = str.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

			if (pos == -1)
				return null;

			var jsonSettings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};

			try
			{
				var obj = JsonConvert.DeserializeObject(str.Substring(pos), jsonSettings);
				return obj as Exception;
			}
			catch
			{
				return null;
			}
		}
	}
}