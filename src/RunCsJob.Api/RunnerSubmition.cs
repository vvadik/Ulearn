using System.ComponentModel;

namespace RunCsJob.Api
{
	public abstract class RunnerSubmition
	{
		public string Id;
		public string Input;
		public bool NeedRun;

		public override string ToString()
		{
			return $"Id: {Id}, NeedRun: {NeedRun}";
		}
	}

	[DisplayName("file")]
	public class FileRunnerSubmition : RunnerSubmition
	{
		public string Code;
	}

	[DisplayName("proj")]
	public class ProjRunnerSubmition : RunnerSubmition
	{
		public byte[] ZipFileData;
		public string ProjectFileName;
	}
}