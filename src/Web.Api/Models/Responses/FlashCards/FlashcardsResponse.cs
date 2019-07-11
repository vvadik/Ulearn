using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Flashcards
{
	[DataContract]
	public class FlashcardsResponse
	{
		public FlashcardsResponse()
		{
			Flashcards = new List<FlashcardResponse>();
		}
		

		[DataMember]
		public List<FlashcardResponse> Flashcards;
	}
}