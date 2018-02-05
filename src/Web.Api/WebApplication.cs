using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Hosting;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging.Serilog;
using Vostok.Logging.Serilog.Enrichers;
using Vostok.Metrics;

namespace Web.Api
{
    public class WebApplication : AspNetCoreVostokApplication
    {
        protected override void OnStarted(IVostokHostingEnvironment hostingEnvironment)
        {
            hostingEnvironment.MetricScope.SystemMetrics(1.Minutes());
        }

        protected override IWebHost BuildWebHost(IVostokHostingEnvironment hostingEnvironment)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<ThreadEnricher>()
                .Enrich.With<FlowContextEnricher>()
                .MinimumLevel.Debug()
                .WriteTo.Airlock(LogEventLevel.Information);
            if (hostingEnvironment.Log != null)
                loggerConfiguration = loggerConfiguration.WriteTo.VostokLog(hostingEnvironment.Log);
            var logger = loggerConfiguration.CreateLogger();
            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{hostingEnvironment.Configuration["port"]}/")
                .AddVostokServices()
				.ConfigureServices(s => ConfigureServices(s, hostingEnvironment, logger))
                .UseSerilog(logger)
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                    app.UseVostok();
                    if (env.IsDevelopment())
                        app.UseDeveloperExceptionPage();
                    app.UseMvc();
                })
                .Build();
        }

		private void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment, Logger logger)
		{
			services.AddDbContext<UlearnDb>(
				options => options.UseSqlServer(hostingEnvironment.Configuration["database"])
			);
			services.AddSingleton(logger);
			
			/* Asp.NET Core MVC */
			services.AddMvc();
		}
	}
}