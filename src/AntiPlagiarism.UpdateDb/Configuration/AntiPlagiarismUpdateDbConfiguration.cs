using AntiPlagiarism.Web.Configuration;
using uLearn.Configuration;

namespace AntiPlagiarism.UpdateDb.Configuration
{
	public class AntiPlagiarismUpdateDbConfiguration: AbstractConfiguration
	{
		public string Database { get; set; }
		
		public HostLogConfiguration HostLog { get; set; }
		
		public AntiPlagiarismConfiguration AntiPlagiarism { get; set; }
	}
}