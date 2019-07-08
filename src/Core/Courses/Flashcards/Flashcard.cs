using System.Xml.Serialization;

namespace Ulearn.Core.Courses.Flashcards
{
	[XmlType("flashcard")]
	public class Flashcard : IFlashcard
	{
		[XmlAttribute("id")]
		public string Id { get; }

		[XmlElement(typeof(FlashcardBlock))]
		public FlashcardBlock[] QuestionBlocks { get; }

		[XmlElement(typeof(FlashcardBlock))]
		public FlashcardBlock[] AnswerBlocks { get; }
	}
}