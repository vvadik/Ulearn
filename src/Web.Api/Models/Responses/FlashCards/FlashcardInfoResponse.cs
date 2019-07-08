using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashcardInfoResponse
	{
		[DataMember]
		public List<FlashcardsInfo> FlashcardsInfos = new List<FlashcardsInfo>();

		public void Add(FlashcardsInfo flashcardsInfo)
		{
			FlashcardsInfos.Add(flashcardsInfo);
		}
	}
}