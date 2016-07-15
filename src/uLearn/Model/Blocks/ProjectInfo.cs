using System.Xml.Serialization;

namespace uLearn.Model.Blocks
{
    ///<summary>Отметка в коде</summary>
    public class ProjectInfo
    {
        [XmlAttribute("project-name")]
        public string ProjectName { get; set; }

        [XmlAttribute("project-path")]
        public string Path { get; set; }
    }
}