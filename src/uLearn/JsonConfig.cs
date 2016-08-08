using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using RunCsJob.Api;

namespace uLearn
{
	public static class JsonConfig
	{
		public static JsonSerializerSettings GetSettings()
		{
			return new JsonSerializerSettings
			{
				Binder = new DisplayNameSerializationBinder(
					Assembly
						.GetAssembly(typeof(RunnerSubmition))
						.GetTypes()
						.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(RunnerSubmition)))),
				TypeNameHandling = TypeNameHandling.Auto,
			};
		}
	}
}