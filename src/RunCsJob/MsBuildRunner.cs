using System.IO;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using RunCsJob.Api;

namespace RunCsJob
{
    public class MsBuildRunner
    {
        public MSBuildResult BuildProject(ProjRunnerSubmition submition)
        {
            var result = new MSBuildResult();
            var path = Path.Combine(".", submition.Id, submition.ProjectFileName);
            var proj = new Project(path);
            var logMessageBuilder = new StringBuilder();
            var stringWriter = new StringWriter(logMessageBuilder);
            var logger = new ConsoleLogger(LoggerVerbosity.Minimal, stringWriter.Write, color => { }, () => { });
            result.Success = proj.Build(logger);
            if (result.Success)
            {
                var exeFileName = Path.GetFileName(submition.ProjectFileName).Substring(0, submition.ProjectFileName.Length - 7);
                result.PathToExe = Path.Combine(".", submition.Id, "bin", "debug", exeFileName);
            }
            else
                result.ErrorMessage = logMessageBuilder.ToString();
            return result;
        }
    }
}