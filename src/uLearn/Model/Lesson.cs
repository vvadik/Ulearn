using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	[XmlType(IncludeInSchema = false)]
	public enum BlockTypes
	{
		[XmlEnum("youtube")]
		YouTube,
		[XmlEnum("md")]
		Md,
		[XmlEnum("code")]
		Code,
		[XmlEnum("tex")]
		Tex,
		[XmlEnum("gallery-images")]
		GalleryImages,
		[XmlEnum("include-code")]
		IncludeCode,
		[XmlEnum("include-md")]
		IncludeMd,
		[XmlEnum("gallery")]
		IncludeImageGalleryBlock,
		[XmlEnum("exercise")]
		ExerciseBlock,
		[XmlEnum("execirse")]
		ExecirseBlock
	}

	[XmlRoot("Lesson", IsNullable = false, Namespace = "https://ulearn.azurewebsites.net/lesson")]
	public class Lesson
	{
		[XmlElement("title")]
		public string Title;

		[XmlElement("id")]
		public string Id;
		
		[XmlElement("default-include-file")]
		public string DefaultInclideFile;

		[XmlElement("default-include-code-file")]
		public string DefaultIncludeCodeFile
		{
			get { return DefaultInclideFile;  }
			set { DefaultInclideFile = value;  }
		}

		[XmlElement(typeof(YoutubeBlock))]
		[XmlElement(typeof(MdBlock))]
		[XmlElement(typeof(CodeBlock))]
		[XmlElement(typeof(TexBlock))]
		[XmlElement(typeof(ImageGaleryBlock))]
		[XmlElement(typeof(IncludeCodeBlock))]
		[XmlElement(typeof(IncludeMdBlock))]
		[XmlElement(typeof(IncludeImageGalleryBlock))]
		[XmlElement(typeof(ExerciseBlock))]
		[XmlElement("execirse", typeof(ExerciseBlock))]
		[XmlChoiceIdentifier("DefineBlockType")]
		public SlideBlock[] Blocks;

		[XmlIgnore]
		public BlockTypes[] DefineBlockType;

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
