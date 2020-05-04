using System;
using System.IO;
using Autofac;
using CommandLine;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static IContainer container;

		private static void Main(string[] args)
		{
			ConfigureAutofac();

			Parser.Default.ParseArguments<Options>(args).WithParsed(Process);

			Console.WriteLine("Press 'q' to quit");
			while (Console.Read() != 'q')
			{
			}
		}

		private static void ConfigureAutofac()
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<CourseUpdateQuery>().As<ICourseUpdateQuery>().SingleInstance();
			containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();
			containerBuilder.RegisterType<UlearnApiClient>().As<IUlearnApiClient>().SingleInstance();
			containerBuilder.RegisterType<CourseUpdateSender>().As<ICourseUpdateSender>().SingleInstance();
			containerBuilder.RegisterType<CourseWatcher>().As<ICourseWatcher>().SingleInstance();
			container = containerBuilder.Build();
		}

		private static void Process(Options options)
		{
			var config = container.Resolve<IConfig>();

			config.Path = Directory.GetCurrentDirectory();
			config.CourseId = options.CourseId;

			var loginPasswordParameters = new LoginPasswordParameters
			{
				Login = options.Login,
				Password = options.Password
			};

			var getJwtTokenTask = HttpMethods.GetJwtToken(loginPasswordParameters);
			config.JwtToken = getJwtTokenTask.Result; // todo

			if (!options.CourseIdAlreadyExist)
			{
				var createCourseTask = HttpMethods.CreateCourse(config.JwtToken.Token, config.CourseId);
				createCourseTask.Wait();
			}

			container.Resolve<IUlearnApiClient>().SendFullCourse(config.Path, config.JwtToken.Token, config.CourseId);

			var sendFullArchive = options.SendFullArchive;
			container.Resolve<ICourseWatcher>().StartWatch(sendFullArchive);
		}
	}
}