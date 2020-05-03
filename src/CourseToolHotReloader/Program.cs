using System;
using System.IO;
using Autofac;
using CommandLine;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static IContainer container;

		private static void Main(string[] args)
		{
			//TestMain();
			//ZipTest();

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
			//containerBuilder.RegisterModule(new ControllerDependencyModule()); //todo починить модуль 
			containerBuilder.RegisterType<CourseUpdateQuery>().As<ICourseUpdateQuery>().SingleInstance();
			containerBuilder.RegisterType<UlearnApiClient>().As<IUlearnApiClient>().SingleInstance();
			containerBuilder.RegisterType<CourseUpdateSender>().As<ICourseUpdateSender>().SingleInstance();
			containerBuilder.RegisterType<CourseWatcher>().As<ICourseWatcher>().SingleInstance();
			container = containerBuilder.Build();
		}

		private static void Process(Options options)
		{
			var patToCourse = options.Path;
			container.Resolve<ICourseWatcher>().StartWatch(patToCourse);
		}

		// пока нет тестов тестирую как могу
		private static void TestMain()
		{
			HttpMethods.TestCreateCourse();
		}

		// пока нет тестов тестирую как могу
		private static void ZipTest()
		{
			//ZipHelper.CreateNewZipByUpdates();
		}
	}
}