using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using Ionic.Zip;
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

        [XmlElement("require-review")]
        public bool RequireReview { get; set; }

        public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
        {
            FillProperties(context);
            ValidatorName = string.Join(" ", LangId, ValidatorName);
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
                ZipFileData = GetZipFileBytes(code, slideFolderPath),
                ProjectFileName = CsProjFilePath,
                Input = "",
                NeedRun = true
            }; ;
        }

        private byte[] GetZipFileBytes(string code, string slideFolderPath)
        {
            using (var zip = new ZipFile())
            {
                var path = Path.Combine(slideFolderPath, Path.GetDirectoryName(CsProjFilePath));
                zip.AddDirectory(path);
                zip.UpdateEntry(UserCodeFileName, code);
                foreach (var pathToExclude in PathsToExcludeForChecker ?? new string[0])
                {
                    zip.RemoveSelectedEntries(pathToExclude);
                }
                using (var ms = new MemoryStream())
                {
                    zip.Save(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}