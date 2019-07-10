using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Serialization;

namespace Ulearn.Core.Courses.Slides.Flashcards
{
	[XmlRoot("slide.flashcards", IsNullable = false, Namespace = "http://ulearn-test-01.dev.kontur.ru/schema/v2")]
	public class FlashcardSlide : Slide
	{
		[XmlElement(ElementName = "flashcard")]
		public Flashcard[] FlashcardsList { get; set; }

		public override void BuildUp(SlideLoadingContext context)
		{
			if (FlashcardsList is null)
				FlashcardsList = new Flashcard[0];
			foreach (var flashcard in FlashcardsList)
			{
				flashcard.BuildUp(context, this);
			}

			if (Title is null)
			{
				Title = "Флеш-карты";
			}

			base.BuildUp(context);
		}

		public override void Validate(SlideLoadingContext context)
		{
			foreach (var flashcard in FlashcardsList)
			{
				flashcard.Validate(context,this);
			}

			base.Validate(context);
		}
	}
}