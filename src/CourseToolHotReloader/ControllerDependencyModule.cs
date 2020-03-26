using System;
using Autofac;

namespace CourseToolHotReloader
{
    public class ControllerDependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.FullName.StartsWith("ConsoleHotReloader"))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}