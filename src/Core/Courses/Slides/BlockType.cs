using System;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;

namespace Ulearn.Core.Courses.Slides
{
	[XmlType(IncludeInSchema = false)]
	public enum BlockType
	{
		[XmlEnum("youtube")]
		YouTube,

		[XmlEnum("markdown")]
		Markdown,

		[XmlEnum("code")]
		Code,

		[XmlEnum("tex")]
		Tex,

		[XmlEnum("gallery-images")]
		GalleryImages,

		[XmlEnum("include-code")]
		IncludeCode,

		[XmlEnum("include-markdown")]
		IncludeMarkdown,

		[XmlEnum("gallery")]
		IncludeImageGalleryBlock,

		[XmlEnum("single-file-exercise")]
		SingleFileExerciseBlock,

		[XmlEnum("proj-exercise")]
		ProjectExerciseBlock,

		[XmlEnum("annotation")]
		VideoAnnotationBlock,
		
		[XmlEnum("question.isTrue")]
		IsTrueBlock,
		
		[XmlEnum("question.choice")]
		ChoiceBlock,
		
		[XmlEnum("question.text")]
		FillInBlock,
		
		[XmlEnum("question.order")]
		OrderingBlock,
		
		[XmlEnum("question.match")]
		MatchingBlock,
	}

	public static class BlockTypeHelpers
	{
		public static BlockType GetBlockType(SlideBlock block)
		{
			switch (block)
			{
				case YoutubeBlock _: return BlockType.YouTube;
				case CodeBlock _: return BlockType.Code;
				case ImageGalleryBlock _: return BlockType.GalleryImages;
				case IncludeCodeBlock _: return BlockType.IncludeCode;
				case IncludeImageGalleryBlock _: return BlockType.IncludeImageGalleryBlock;
				case IncludeMarkdownBlock _: return BlockType.IncludeMarkdown;
				case MarkdownBlock _: return BlockType.Markdown;
				case TexBlock _: return BlockType.Tex; 
				case VideoAnnotationBlock _: return BlockType.VideoAnnotationBlock;
				
				case FillInBlock _: return BlockType.FillInBlock;
				case ChoiceBlock _: return BlockType.ChoiceBlock;
				case MatchingBlock _: return BlockType.MatchingBlock;
				case OrderingBlock _: return BlockType.OrderingBlock;
				case IsTrueBlock _: return BlockType.IsTrueBlock;
				
				case ProjectExerciseBlock _: return BlockType.ProjectExerciseBlock;
				case SingleFileExerciseBlock _: return BlockType.SingleFileExerciseBlock;
				
				default: throw new Exception("Unknown slide block " + block);
			}
		}
	}
}