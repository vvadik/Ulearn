using System;
using System.Runtime.Serialization;
using AntiPlagiarism.Web.Database;
using Newtonsoft.Json;

namespace ManualUtils.AntiPlagiarism
{
	[DataContract]
	public class AdditionalInfo
	{
		[DataMember(Name = "SubmissionId")]
		public int SubmissionId { get; set; }
	}
	
	public static class FillAntiplagFields
	{
		public static void FillClientSubmissionId(AntiPlagiarismDb adb)
		{
			var id = 1;
			while (true)
			{
				var submission = adb.Submissions.Find(id);
				if (submission == null)
				{
					Console.WriteLine($"Stop on id {id}");
					return;
				}
				if (id % 1000 == 0)
					Console.WriteLine(id);
				try
				{
					submission.ClientSubmissionId = JsonConvert.DeserializeObject<AdditionalInfo>(submission.AdditionalInfo).SubmissionId.ToString();
					adb.Submissions.Update(submission);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on \"{submission.AdditionalInfo}\"");
				}
				id++;
			}
		}
	}
}