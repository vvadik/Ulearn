using System;
using System.IO;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using NUnit.Framework;
using RunCsJob.Api;

namespace RunCsJob
{
    public class MsBuildRunner
    {
        public MSBuildResult BuildProject(ProjRunnerSubmition submition, DirectoryInfo dir)
        {
            var result = new MSBuildResult();
            var path = Path.Combine(dir.FullName, Path.GetFileName(submition.ProjectFileName));
            var proj = new Project(path);
            var stringWriter = new StringWriter();
            var logger = new ConsoleLogger(LoggerVerbosity.Minimal, stringWriter.Write, color => { }, () => { });
            result.Success = proj.Build(logger);
            if (result.Success)
            {
                var csprojFileName = Path.GetFileName(submition.ProjectFileName);
                var exeFileName = csprojFileName.Substring(0, csprojFileName.Length - 7) + ".exe";
                result.PathToExe = Path.Combine(proj.DirectoryPath, "bin", "debug", exeFileName);
            }
            else
                result.ErrorMessage = stringWriter.ToString();
            return result;
        }
    }

    [TestFixture]
    public class MsBuildRunner_Test
    {
        [Test]
        public void Test()
        {
            
        }
    }
}