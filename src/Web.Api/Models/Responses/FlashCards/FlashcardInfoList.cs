using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashcardInfoList
	{
		[DataMember]
		public List<FlashcardsInfo> FlashcardsInfos;
	}
}