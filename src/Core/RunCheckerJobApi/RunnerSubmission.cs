using System.ComponentModel;
using Ulearn.Common;

namespace Ulearn.Core.RunCheckerJobApi
{
	public abstract class RunnerSubmission
	{
		public string Id;
		public int TimeLimit = 10;

		public override string ToString()
		{
			return $"Id: {Id}";
		}
	}

	public abstract class CsRunnerSubmission : RunnerSubmission
	{
		public string Input;
		public bool NeedRun; // Только проверить факт, что компилируется

		public override string ToString()
		{
			return $"Id: {Id}, NeedRun: {NeedRun}";
		}
	}

	[DisplayName("file")]
	public class FileRunnerSubmission : CsRunnerSubmission
	{
		public string Code;
	}

	[DisplayName("proj")]
	public class ProjRunnerSubmission : CsRunnerSubmission
	{
		public byte[] ZipFileData;
		public string ProjectFileName;
	}

	[DisplayName("command")]
	public class CommandRunnerSubmission : RunnerSubmission
	{
		public byte[] ZipFileData;
		public Language Language;
		public string DockerImageName;
		public string RunCommand;
	}
}