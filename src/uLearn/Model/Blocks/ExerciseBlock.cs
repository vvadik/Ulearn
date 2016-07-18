using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
    [XmlType("execirse")]
    public class ExerciseBlock : IncludeCode
    {
        [XmlElement("inital-code")]
        public string ExerciseInitialCode { get; set; }

        [XmlElement("hint")]
        public List<string> Hints { get; set; }

        [XmlElement("solution")]
        public Label SolutionLabel { get; set; }

        [XmlElement("csproj-file-path")]
        public string CSProjFilePath { get; set; }

        [XmlElement("user-sln-file-path")]
        public string UserSlnFilePath { get; set; }

        [XmlElement("remove-file-path")]
        public string[] RemovedFiles { get; set; }

        [XmlElement("remove")]
        public Label[] RemovedLabels { get; set; }

        [XmlElement("comment")]
        public string CommentAfterExerciseIsSolved { get; set; }

        [XmlElement("expected")]
        // Ожидаемый корректный вывод программы
        public string ExpectedOutput { get; set; }

        [XmlElement("hide-expected-output")]
        public bool HideExpectedOutputOnError { get; set; }

        [XmlElement("validator")]
        public string ValidatorName { get; set; }

        [XmlElement("prelude")]
        public string PreludeFile { get; set; }

        public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
        {
            FillProperties(context);
            RemovedLabels = RemovedLabels ?? new Label[0];
            if (PreludeFile == null)
            {
                PreludeFile = context.CourseSettings.GetPrelude(LangId);
                if (PreludeFile != null)
                    PreludeFile = Path.Combine("..", PreludeFile);
            }

            var code = context.FileSystem.GetContent(File);
            var regionRemover = new RegionRemover(LangId);
            var extractor = context.GetExtractor(File, LangId, code);

            var prelude = "";
            if (PreludeFile != null)
                prelude = context.FileSystem.GetContent(PreludeFile);

            var exerciseCode = regionRemover.Prepare(code);
            IEnumerable<Label> notRemoved;
            exerciseCode = regionRemover.Remove(exerciseCode, RemovedLabels, out notRemoved);
            int index;
            exerciseCode = regionRemover.RemoveSolution(exerciseCode, SolutionLabel, out index);
            index += prelude.Length;

            ExerciseInitialCode = ExerciseInitialCode.RemoveCommonNesting();
            ExerciseCode = prelude + exerciseCode;
            IndexToInsertSolution = index;
            EthalonSolution = extractor.GetRegion(SolutionLabel);
            ValidatorName = string.Join(" ", LangId, ValidatorName);

            yield return this;
        }

        // То, что будет выполняться для проверки задания
        public string ExerciseCode { get; set; }
        // Индекс внутри ExerciseCode, куда нужно вставить код пользователя.
        public int IndexToInsertSolution { get; set; }
        // Если это вставить в ExerciseCode по индексу IndexToInsertSolution и выполнить полученный код, он должен вывести ExpectedOutput
        public string EthalonSolution { get; set; }

        public List<string> HintsMd
        {
            get { return Hints = Hints ?? new List<string>(); }
            set { Hints = value; }
        }

        [XmlIgnore]
        public SolutionBuilder Solution
        {
            get { return new SolutionBuilder(IndexToInsertSolution, ExerciseCode, ValidatorName); }
        }

        #region equals

        protected bool Equals(ExerciseBlock other)
        {
            return Equals(ExerciseInitialCode, other.ExerciseInitialCode) && Equals(ExpectedOutput, other.ExpectedOutput) && Equals(HintsMd, other.HintsMd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ExerciseBlock)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (ExerciseInitialCode != null ? ExerciseInitialCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ExpectedOutput != null ? ExpectedOutput.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HintsMd != null ? HintsMd.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Exercise: {0}, Hints: {1}", ExerciseInitialCode, string.Join("; ", HintsMd));
        }

        public Component GetSolutionsComponent(string displayName, Slide slide, int componentIndex, string launchUrl, string ltiId)
        {
            return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex + 1, launchUrl, ltiId, false, 0, false);
        }

        public Component GetExerciseComponent(string displayName, Slide slide, int componentIndex, string launchUrl, string ltiId)
        {
            return new LtiComponent(displayName, slide.NormalizedGuid + componentIndex, launchUrl, ltiId, true, 5, false);
        }

        public override Component ToEdxComponent(string displayName, Slide slide, int componentIndex)
        {
            throw new NotImplementedException();
        }

        public override string TryGetText()
        {
            return (ExerciseInitialCode ?? "") + '\n'
                   + string.Join("\n", HintsMd) + '\n'
                   + (CommentAfterExerciseIsSolved ?? "");
        }
    }
}