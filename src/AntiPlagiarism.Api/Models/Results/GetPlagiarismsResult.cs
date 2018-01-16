using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class GetPlagiarismsResult : ApiSuccessResult
	{
		[DataMember(Name = "plagiarisms")]
		public List<Plagiarism> Plagiarisms { get; set; }

		public GetPlagiarismsResult()
		{
			Plagiarisms = new List<Plagiarism>();
		}
	}

	[DataContract]
	public class Plagiarism
	{
		[DataMember(Name = "submission")]
		public PlagiateSubmission Submission { get; set; }
		
		[DataMember(Name = "weight")]
		public double Weight { get; set; }
	}

	[DataContract]
	public class PlagiateSubmission
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		
		[DataMember(Name = "code")]
		public string Code { get; set; }
		
		[DataMember(Name = "task_id")]
		public Guid TaskId { get; set; }
		
		[DataMember(Name = "author_id")]
		public Guid AuthorId { get; set; }

		[DataMember(Name = "additional_info")]
		public string AdditionalInfo;
	}
}