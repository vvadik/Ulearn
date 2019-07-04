using System;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashcardsInfo
	{
		[DataMember]
		public string UnitTitle;

		[DataMember]
		public bool Unlocked;

		[DataMember]
		public int CardsCount;

		[DataMember]
		public Guid UnitId;
	}
}