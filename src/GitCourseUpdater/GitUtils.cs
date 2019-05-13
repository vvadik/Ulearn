
namespace GitCourseUpdater
{
	public class GitUtils
	{
		public static string RepoUrlToBrowserLink(string repoUrl, string hash)
		{
			var link = repoUrl.Substring(4);
			link = repoUrl.Substring(0, repoUrl.Length - 4);
			link = link.Replace(':', '/');
			return $"https://{link}/commit/{hash}";
		}
	}
}