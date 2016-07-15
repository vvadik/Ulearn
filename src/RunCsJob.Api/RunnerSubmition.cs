namespace RunCsJob.Api
{
    public abstract class RunnerSubmition
    {
        public string Id;
        public string Input;
        public bool NeedRun;

        public override string ToString()
        {
            return string.Format("Id: {0}, NeedRun: {1}", Id, NeedRun);
        }
    }

    public class FileRunnerSubmition : RunnerSubmition
    {
        public string Code;
    }


    public class ProjRunnerSubmition : RunnerSubmition
    {
        public string ZipFileData;

        public string ProjectFileName
        {
            get { return ProjectName + ".csproj"; }
        }

        public string ProjectName;
    }
}