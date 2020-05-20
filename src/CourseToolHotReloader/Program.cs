using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.Authorize;
using CourseToolHotReloader.DirectoryWorkers;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static IContainer container;

		private static void Main(string[] args)
		{
			container = ConfigureAutofac.Build();

			Parser.Default.ParseArguments<Options>(args).WithParsed(ParseOption);

			Console.WriteLine("Press 'q' to quit");
			while (Console.Read() != 'q')
			{
			}
		}


		private static void ParseOption(Options options)
		{
			var task = ParseOptionAsync(options);
			task.Wait();
		}

		private static async Task ParseOptionAsync(Options options)
		{
			await container.Resolve<IAuthorizer>().SignIn(); // нужно вынести

			var config = container.Resolve<IConfig>();

			config.Path = Directory.GetCurrentDirectory();
			config.CourseId = options.CourseId;

			if (!options.CourseIdAlreadyExist)
			{
				await HttpMethods.CreateCourse(config.JwtToken.Token, config.CourseId);
			}

			await container.Resolve<IUlearnApiClient>().SendFullCourse(config.Path, config.JwtToken.Token, config.CourseId);

			var sendFullArchive = options.SendFullArchive;
			container.Resolve<ICourseWatcher>().StartWatch(sendFullArchive);
		}
	}
}