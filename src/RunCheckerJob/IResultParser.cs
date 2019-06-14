using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public interface IResultParser
	{
		RunningResults Parse(string json);
	}
}