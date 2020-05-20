using Autofac;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.UpdateQuery;

namespace CourseToolHotReloader
{
	public static class ConfigureAutofac
	{
		public static IContainer Build()
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<CourseUpdateQuery>().As<ICourseUpdateQuery>().SingleInstance();
			containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();
			containerBuilder.RegisterType<UlearnApiClient>().As<IUlearnApiClient>().SingleInstance();
			containerBuilder.RegisterType<CourseUpdateSender>().As<ICourseUpdateSender>().SingleInstance();
			containerBuilder.RegisterType<CourseWatcher>().As<ICourseWatcher>().SingleInstance();
			containerBuilder.RegisterType<SendFullCourseStrategy>().As<ISendFullCourseStrategy>().SingleInstance();
			containerBuilder.RegisterType<SendOnlyChangedStrategy>().As<ISendOnlyChangedStrategy>().SingleInstance();
			return containerBuilder.Build();
		}
			
	}
}