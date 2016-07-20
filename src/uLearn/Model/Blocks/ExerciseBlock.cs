using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
    public abstract class ExerciseBlock : IncludeCode
    {
        [XmlElement("inital-code")]
        public string ExerciseInitialCode { get; set; }

        [XmlElement("hint")]
        public List<string> Hints { get; set; }

        [XmlElement("comment")]
        public string CommentAfterExerciseIsSolved { get; set; }

        [XmlElement("expected")]
        // Ожидаемый корректный вывод программы
        public string ExpectedOutput { get; set; }

        [XmlElement("hide-expected-output")]
        public bool HideExpectedOutputOnError { get; set; }

        [XmlElement("validator")]
        public string ValidatorName { get; set; }

        [XmlElement("hide-show-solutions-button")]
        public bool HideShowSolutionsButton { get; set; }

        public List<string> HintsMd
        {
            get { return Hints = Hints ?? new List<string>(); }
            set { Hints = value; }
        }

        public abstract string GetSourceCode(string code);

        public abstract SolutionBuildResult BuildSolution(string code);
        
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
            throw new NotSupportedException();
        }

        public override string TryGetText()
        {
            return (ExerciseInitialCode ?? "") + '\n'
                   + string.Join("\n", HintsMd) + '\n'
                   + (CommentAfterExerciseIsSolved ?? "");
        }
    }
}