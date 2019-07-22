using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Flashcards;

namespace Ulearn.Web.Api.Models.Responses.FlashCards
{
	[DataContract]
	public class FlashcardResponseByUnits
	{
		[DataMember]
		public List<UnitFlashcardsResponse> Units = new List<UnitFlashcardsResponse>();

	}
}