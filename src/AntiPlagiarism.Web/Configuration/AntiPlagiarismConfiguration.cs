namespace AntiPlagiarism.Web.Configuration
{
	public class AntiPlagiarismConfiguration
	{
		public int SnippetTokensCount { get; set; }
		public int MaxCodeLength { get; set; }
		public StatisticsAnalyzingConfiguration StatisticsAnalyzing { get; set; }
	}
	
	public class StatisticsAnalyzingConfiguration
	{
		public int CountOfLastAuthorsForCalculatingMeanAndDeviation { get; set; }
	}
}