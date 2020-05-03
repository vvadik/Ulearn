using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("spoiler")]
	[DataContract]
	public class SpoilerBlock : SlideBlock, IApiSlideBlock, IApiConvertibleSlideBlock
	{
		[XmlAttribute("text")]
		[DataMember]
		public string Text { get; set; }

		[XmlAttribute("hideQuizButton")]
		[DataMember(Name = "hideQuizButton", EmitDefaultValue = false)]
		public bool HideQuizButton { get; set; }

		[XmlElement(typeof(YoutubeBlock))]
		[XmlElement("markdown", typeof(MarkdownBlock))]
		[XmlElement(typeof(CodeBlock))]
		[XmlElement(typeof(TexBlock))]
		[XmlElement(typeof(ImageGalleryBlock))]
		[XmlElement(typeof(IncludeCodeBlock))]
		[XmlElement(typeof(IncludeMarkdownBlock))]
		[XmlElement(typeof(IncludeImageGalleryBlock))]
		[XmlElement("html", typeof(HtmlBlock))]
		[XmlChoiceIdentifier(nameof(DefineBlockType))]
		[IgnoreDataMember]
		public SlideBlock[] Blocks { get; set; }

		[XmlIgnore]
		[DataMember(Name = "blocks")]
		public List<IApiSlideBlock> ApiBlocks { get; set; } // Заполняется в ToApiSlideBlocks

		[XmlIgnore]
		[IgnoreDataMember]
		public BlockType[] DefineBlockType;

		public override string ToString()
		{
			return $"Spoiler {Text}";
		}

		IEnumerable<IApiSlideBlock> IApiConvertibleSlideBlock.ToApiSlideBlocks(ApiSlideBlockBuildingContext context)
		{
			var clone = (SpoilerBlock)MemberwiseClone();
			clone.ApiBlocks = Blocks.SelectMany(b => b.ToApiSlideBlocks(context)).ToList();
			yield return clone;
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			if (Blocks == null)
				Blocks = new SlideBlock[0];

			Blocks = Blocks.SelectMany(b => b.BuildUp(context, filesInProgress)).ToArray();

			DefineBlockTypes();

			return base.BuildUp(context, filesInProgress);
		}

		public void DefineBlockTypes()
		{
			if (Blocks != null)
				DefineBlockType = Blocks.Select(BlockTypeHelpers.GetBlockType).ToArray();
		}

		public override void Validate(SlideBuildingContext slideBuildingContext)
		{
			if (HideQuizButton && !(slideBuildingContext.Slide is QuizSlide))
				throw new CourseLoadingException("У блока <spoiler> указан атрибут hideQuizButton=\"true\", хотя слайд не является тестом. Этот атрибут можно использовать только внутри <slide.quiz>");
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new System.NotImplementedException();
		}

		[XmlIgnore]
		[DataMember(Name = "type")]
		public string Type { get; set; } = "spoiler";
	}
}