using Microsoft.Build.Evaluation;

namespace uLearn
{
	public static class ProjectCollectionHelper
	{
		public static ProjectCollection CreateEmptyProjectCollection()
		{
			return new ProjectCollection(ToolsetDefinitionLocations.Registry | ToolsetDefinitionLocations.ConfigurationFile);
		}
	}
}