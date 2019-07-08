using System;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Flashcards
{
	[XmlRoot("slide", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class Flashcards : IFlashcards
	{
		[XmlAttribute("id")]

		private Guid id;

		[XmlElement]
		public Flashcard[] FlashcardsList { get; }


		Guid IFlashcards.Id => id;
	}
}