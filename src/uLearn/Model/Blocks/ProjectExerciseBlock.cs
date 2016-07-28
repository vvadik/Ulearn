using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RunCsJob.Api;

namespace uLearn.Model.Blocks
{
    [XmlType("proj-exercise")]
    public class ProjectExerciseBlock : ExerciseBlock
    {
        [XmlElement("csproj-file-path")]
        public string CsProjFilePath { get; set; }

        [XmlElement("user-code-file-name")]
        public string UserCodeFileName { get; set; }

        [XmlElement("exclude-path-for-checker")]
        public string[] PathsToExcludeForChecker { get; set; }

        [XmlElement("exclude-path-for-student")]
        public string[] PathsToExcludeForStudent { get; set; }

        [XmlElement("require-review")]
        public bool RequireReview { get; set; }

        private string ExerciseDir => Path.GetDirectoryName(CsProjFilePath).EnsureNotNull("csproj должен быть в поддиректории");

        private string CsprojFileName => Path.GetFileName(CsProjFilePath);

        public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
        {
            FillProperties(context);
            ValidatorName = string.Join(" ", LangId, ValidatorName);
            var directoryName = Path.Combine(context.Dir.FullName, ExerciseDir);
            var excluded = (PathsToExcludeForStudent ?? new string[0]).Concat(new[] { "checker/", "bin/", "obj/" });
            var csprojFileName = CsprojFileName;
            var zipData = context.Dir.GetSubdir(ExerciseDir).ZipTo(excluded, new[]
            {
                new ZipUpdateData
                {
                    Path = csprojFileName,
                    Data = ProjModifier.ModifyCsproj(context.Dir.GetBytes(CsProjFilePath), ProjModifier.RemoveCheckingFromCsproj)
                }
            });
            System.IO.File.WriteAllBytes(directoryName + ".zip", zipData);
            yield return this;
        }

        public override string GetSourceCode(string code)
        {
            return code;
        }

        public override SolutionBuildResult BuildSolution(string code)
        {
            var validator = ValidatorsRepository.Get(ValidatorName);
            return validator.ValidateSolution(code);
        }

        public override RunnerSubmition CreateSubmition(string submitionId, string code, string slideFolderPath)
        {
            return new ProjRunnerSubmition
            {
                Id = submitionId,
                ZipFileData = GetZipBytesForChecker(code, slideFolderPath),
                ProjectFileName = CsProjFilePath,
                Input = "",
                NeedRun = true
            };
        }

        private byte[] GetZipBytesForChecker(string code, string slideFolderPath)
        {
            var directoryName = Path.Combine(slideFolderPath, ExerciseDir);
            var excluded = (PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/", "obj/" });
            var exerciseDir = new DirectoryInfo(directoryName);
            return exerciseDir.ZipTo(excluded,
                new[]
                {
                    new ZipUpdateData { Path = CsprojFileName, Data = Encoding.UTF8.GetBytes(code) },
                    new ZipUpdateData { Path = CsprojFileName, Data = ProjModifier.ModifyCsproj(exerciseDir.GetBytes(CsprojFileName), ProjModifier.ChangeEntryPointToCheckingCheckerMain) }
                });
        }
    }
}