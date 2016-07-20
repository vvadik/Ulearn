using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;

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

        [XmlElement("publish-in-rb")]
        public bool PublishInRb { get; set; }

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
    }
}