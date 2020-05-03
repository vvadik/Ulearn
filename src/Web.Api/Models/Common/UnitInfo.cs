using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class UnitInfo
	{
		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public List<ShortSlideInfo> Slides { get; set; }
		
		[DataMember]
		public List<UnitScoringGroupInfo> AdditionalScores { get; set; }
	}
}