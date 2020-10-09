using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class SubmissionInfo
	{
		[DataMember(Name="id")]
		public int Id;

		[DataMember(Name="code")]
		public string Code;

		[DataMember(Name="timestamp")]
		public DateTime Timestamp;

		[DataMember(Name="reviews")]
		public List<ReviewInfo> Reviews;
		
		[DataMember(Name="output")]
		public string Output;

		[DataMember(Name="points")]
		public float Points;
	}
}