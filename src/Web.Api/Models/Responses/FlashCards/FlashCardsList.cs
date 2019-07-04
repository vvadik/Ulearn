using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashCardsList
	{
		[DataMember]
		public List<FlashCard> FlashCards;

	}
}