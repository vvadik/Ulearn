using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Ionic.Zip;
using RunCsJob.Api;

namespace RunCsJob
{
    public class SandboxRunner
    {
        private readonly RunnerSubmition submition;

        private const int TimeLimitInSeconds = 10;
        private static readonly TimeSpan TimeLimit = TimeSpan.FromSeconds(TimeLimitInSeconds);
        private static readonly TimeSpan IdleTimeLimit = TimeSpan.FromSeconds(2 * TimeLimitInSeconds);

        private const int MemoryLimit = 64 * 1024 * 1024;
        private const int OutputLimit = 10 * 1024 * 1024;

        private bool _hasTimeLimit;
        private bool _hasMemoryLimit;
        private bool _hasOutputLimit;

        private readonly RunningResults _result = new RunningResults();

        public static RunningResults Run(RunnerSubmition submition)
        {
            var runnerSubmition = submition as ProjRunnerSubmition;
            if (runnerSubmition != null)
            {
                var projSubmition = runnerSubmition;
                var tempProjectDir = UnpackZip(projSubmition);

                try
                {
                    return new SandboxRunner(submition).RunMSBuild(tempProjectDir);
                }
                catch (Exception ex)
                {
                    return new RunningResults
                    {
                        Id = runnerSubmition.Id,
                        Verdict = Verdict.SandboxError,
                        Error = ex.ToString()
                    };
                }
            }
            try
            {
                return new SandboxRunner(submition).RunCSC();
            }
            catch (Exception ex)
            {
                return new RunningResults
                {
                    Id = submition.Id,
                    Verdict = Verdict.SandboxError,
                    Error = ex.ToString()
                };
            }
        }

        private static DirectoryInfo UnpackZip(ProjRunnerSubmition submition)
        {
            var pathName = Path.Combine(".", submition.Id);
            using (var ms = new MemoryStream(submition.ZipFileData))
            {
                using (var zip = ZipFile.Read(ms))
                {
                    var dir = Directory.CreateDirectory(pathName);
                    foreach (var file in zip)
                        file.Extract(pathName, ExtractExistingFileAction.OverwriteSilently);
                    return dir;
                }
            }
        }

        public SandboxRunner(RunnerSubmition submition)
        {
            this.submition = submition;
            _result.Id = submition.Id;
        }

        public RunningResults RunMSBuild(DirectoryInfo dir)
        {
            var builder = new MsBuildRunner();
            var builderResult = builder.BuildProject((ProjRunnerSubmition)submition, dir);
            if (!builderResult.Success)
            {
                _result.Verdict = Verdict.CompilationError;
                _result.CompilationOutput = builderResult.ErrorMessage;
                Remove(dir.FullName);
                return _result;
            }
            RunSandboxer(string.Format("\"{0}\" {1}", builderResult.PathToExe, submition.Id));

            Remove(dir.FullName);

            _result.Verdict = Verdict.Ok;
            _result.FillPassProgress();

            return _result;
        }

        public RunningResults RunCSC()
        {
            var assemblyCreator = new AssemblyCreator();
            var assembly = assemblyCreator.CreateAssembly((FileRunnerSubmition)submition);

            _result.Verdict = Verdict.Ok;

            _result.AddCompilationInfo(assembly);

            if (_result.IsCompilationError())
            {
                Remove(assembly.PathToAssembly);
                return _result;
            }

            if (!submition.NeedRun)
            {
                Remove(assembly.PathToAssembly);
                return _result;
            }
            RunSandboxer(string.Format("\"{0}\" {1}", Path.GetFullPath(assembly.PathToAssembly), submition.Id));

            Remove(assembly.PathToAssembly);
            return _result;
        }

        private static void Remove(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(Path.GetExtension(path)))
                    Directory.Delete(path, true);
                else
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
                _result.Verdict = Verdict.SandboxError;
                _result.Error = "Can't start proces";
                return;
            }

            var stderrReader = new AsyncReader(sandboxer.StandardError, OutputLimit + 1);

            var readyState = sandboxer.StandardOutput.ReadLineAsync();
            if (!readyState.Wait(TimeLimit) || readyState.Result != "Ready")
            {
                if (!sandboxer.HasExited)
                {
                    sandboxer.Kill();
                    _result.Verdict = Verdict.SandboxError;
                    _result.Error = "Sandbox does not respond";
                    return;
                }
                if (sandboxer.ExitCode != 0)
                {
                    HandleNonZeroExitCode(stderrReader.GetData(), sandboxer.ExitCode);
                    return;
                }
                _result.Verdict = Verdict.SandboxError;
                _result.Error = "Sandbox exit before respond";
                return;
            }

            sandboxer.Refresh();
            var startUsedMemory = sandboxer.WorkingSet64;
            var startUsedTime = sandboxer.TotalProcessorTime;
            var startTime = DateTime.Now;

            sandboxer.StandardInput.WriteLine("Run");
            sandboxer.StandardInput.WriteLineAsync(submition.Input);

            var stdoutReader = new AsyncReader(sandboxer.StandardOutput, OutputLimit + 1);
            while (!sandboxer.HasExited
                   && !IsTimeLimitExpected(sandboxer, startTime, startUsedTime)
                   && !IsMemoryLimitExpected(sandboxer, startUsedMemory)
                   && !IsOutputLimit(stdoutReader)
                   && !IsOutputLimit(stderrReader))
            {
            }

            if (!sandboxer.HasExited)
                sandboxer.Kill();

            if (_hasOutputLimit)
            {
                _result.Verdict = Verdict.OutputLimit;
                return;
            }

            if (_hasTimeLimit)
            {
                _result.Verdict = Verdict.TimeLimit;
                return;
            }

            if (_hasMemoryLimit)
            {
                _result.Verdict = Verdict.MemoryLimit;
                return;
            }

            sandboxer.WaitForExit();
            if (sandboxer.ExitCode != 0)
            {
                HandleNonZeroExitCode(stderrReader.GetData(), sandboxer.ExitCode);
                return;
            }

            _result.Output = stdoutReader.GetData();
            _result.Error = stderrReader.GetData();
        }

        private void HandleNonZeroExitCode(string error, int exitCode)
        {
            var obj = FindSerializedException(error);

            if (obj != null)
                _result.HandleException(obj);
            else
                HandleNtStatus(exitCode, error);
        }

        private void HandleNtStatus(int exitCode, string error)
        {
            switch ((uint)exitCode)
            {
                case 0xC00000FD:
                    _result.Verdict = Verdict.RuntimeError;
                    _result.Error = "Stack overflow exception.";
                    break;
                default:
                    _result.Verdict = Verdict.SandboxError;
                    _result.Error = string.IsNullOrWhiteSpace(error) ? "Non-zero exit code" : error;
                    _result.Error += string.Format("\nExit code: 0x{0:X8}", exitCode);
                    break;
            }
        }

        private bool IsOutputLimit(AsyncReader reader)
        {
            return _hasOutputLimit = _hasOutputLimit
                                     || (reader.ReadedLength > OutputLimit);
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
                return _hasMemoryLimit;
            }

            return _hasMemoryLimit = _hasMemoryLimit
                                     || startUsedMemory + MemoryLimit < mem;
        }

        private bool IsTimeLimitExpected(Process sandboxer, DateTime startTime, TimeSpan startUsedTime)
        {
            return _hasTimeLimit = _hasTimeLimit
                                   || TimeLimit.Add(startUsedTime).CompareTo(sandboxer.TotalProcessorTime) < 0
                                   || startTime.Add(IdleTimeLimit).CompareTo(DateTime.Now) < 0;
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