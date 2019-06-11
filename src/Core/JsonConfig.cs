using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Ulearn.Core.RunCheckerJobApi;

namespace Ulearn.Core
{
	public static class JsonConfig
	{
		public static JsonSerializerSettings GetSettings()
		{
			return new JsonSerializerSettings
			{
				SerializationBinder = new DisplayNameSerializationBinder(
					Assembly
						.GetAssembly(typeof(RunnerSubmission))
						.GetTypes()
						.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(RunnerSubmission)))),
				TypeNameHandling = TypeNameHandling.Auto,
			};
		}
	}
}