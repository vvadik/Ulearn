using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Slides.Flashcards
{
	[XmlType("Flashcard", Namespace = "https://ulearn.me/schema/v2")]
	public class Flashcard : IFlashcard
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "theorySlideId")]
		public Guid[] TheorySlidesIds { get; set; }

		[XmlElement(ElementName = "question")]
		public FlashcardContent Question { get; set; }

		[XmlElement(ElementName = "answer")]
		public FlashcardContent Answer { get; set; }

		public void BuildUp(SlideLoadingContext context, Slide flashcardSlide)
		{
			if (Answer is null)
			{
				Answer = new FlashcardContent();
			}

			if (Question is null)
			{
				Question = new FlashcardContent();
			}

			if (Answer.Blocks is null)
			{
				Answer.Blocks = new SlideBlock[0];
			}

			if (Question.Blocks is null)
			{
				Question.Blocks = new SlideBlock[0];
			}

			var slideLoadingContext = new SlideBuildingContext(context, flashcardSlide);
			Answer.Blocks = Answer.Blocks
				.SelectMany(x => x.BuildUp(slideLoadingContext, ImmutableHashSet<string>.Empty))
				.ToArray();
			Question.Blocks = Question.Blocks
				.SelectMany(x => x.BuildUp(slideLoadingContext, ImmutableHashSet<string>.Empty))
				.ToArray();
		}

		public void Validate(SlideLoadingContext context, Slide flashcardSlide)
		{
			var slideBuildingContext = new SlideBuildingContext(context, flashcardSlide);
			foreach (var block in Question.Blocks)
			{
				block.Validate(slideBuildingContext);
			}

			foreach (var block in Answer.Blocks)
			{
				block.Validate(slideBuildingContext);
			}
		}
	}
}