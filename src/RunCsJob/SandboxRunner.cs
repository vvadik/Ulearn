using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Build.BuildEngine;
using Newtonsoft.Json;
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
            if (submition is ProjRunnerSubmition)
            {
                var projSubmition = (ProjRunnerSubmition)submition;
                UnpackZip(projSubmition);
                try
                {
                    new SandboxRunner(submition).RunMSBuild();
                }
                catch (Exception)
                {
                    
                    throw;
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

        private static void UnpackZip(ProjRunnerSubmition submition)
        {
            var pathName = string.Format(@".\{0}", submition.Id);
            var file = File.Open(pathName + ".zip", FileMode.Create);
            new BinaryWriter(file).Write(submition.ZipFileData);
            Directory.CreateDirectory(pathName);
            ZipFile.ExtractToDirectory(pathName + ".zip", pathName);
            file.Close();
            Remove(pathName + ".zip");
        }

        public SandboxRunner(RunnerSubmition submition)
        {
            this.submition = submition;
            _result.Id = submition.Id;
        }

        public RunningResults RunMSBuild()
        {

            var pathToExe = MsBuildRunner.BuildProject((ProjRunnerSubmition)submition);
            RunSandboxer(pathToExe);//todo add path to .exe
            return _result;
        }

        public RunningResults RunCSC()
        {
            var assembly = AssemblyCreator.CreateAssembly((FileRunnerSubmition)submition);

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