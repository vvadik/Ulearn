using Microsoft.Build.Evaluation;
using RunCsJob.Api;

namespace RunCsJob
{
    public static class MsBuildRunner
    {
        public static string BuildProject(ProjRunnerSubmition submition)
        {
            var path = string.Format(@".\{0}\{1}", submition.Id, submition.ProjectFileName);
            var proj = new Project(path);
            //todo возможно добавить логгер
            proj.Build();
            return string.Format(@".\{0}\bin\Debug\{1}.exe", submition.Id, submition.ProjectName);
        }
    }
}