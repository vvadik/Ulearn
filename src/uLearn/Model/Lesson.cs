using System.Xml.Serialization;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	[XmlRoot("Lesson", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/lesson")]
	public class Lesson
	{
		[XmlElement("title")]
		public string Title;

		[XmlElement("id")]
		public string Id;
		
		[XmlElement("default-include-file")]
		public string DefaultIncludeFile;

		[XmlElement(typeof(YoutubeBlock))]
		[XmlElement("md", typeof(MdBlock))]
		[XmlElement(typeof(CodeBlock))]
		[XmlElement(typeof(TexBlock))]
		[XmlElement(typeof(ImageGaleryBlock))]
		[XmlElement(typeof(IncludeCodeBlock))]
		[XmlElement(typeof(IncludeMdBlock))]
		[XmlElement(typeof(IncludeImageGalleryBlock))]
		[XmlElement(typeof(ProjectExerciseBlock))]
        [XmlElement(typeof(SingleFileExerciseBlock))]
		public SlideBlock[] Blocks;

		public Lesson()
		{
		}

		public Lesson(string title, string id, SlideBlock[] blocks)
		{
			Title = title;
			Id = id;
			Blocks = blocks;
		}
	}
}
