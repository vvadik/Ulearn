using System;
using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Parameters
{
	[DataContract]
	public class AddCodeApiParameters : ApiParameters
	{
		[DataMember(Name = "task_id", IsRequired = true)]
		public Guid TaskId { get; set; }
		
		[DataMember(Name = "author_id", IsRequired = true)]
		public Guid AuthorId { get; set; }
		
		[DataMember(Name = "code", IsRequired = true)]
		public string Code { get; set; }
		
		[DataMember(Name = "additional_info")]
		public string AdditionalInfo { get; set; }

		public override string ToString()
		{
			return $"AddCodeParameters(TaskId={TaskId}, AuthorId={AuthorId}, Code={Code?.Substring(0, Math.Min(Code.Length, 50))?.Replace("\n", @"\\")}...)";
		}
	}
}