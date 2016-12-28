using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Atmosphere.Domain;

namespace Atmosphere.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            var builder = new ContainerBuilder();

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            builder.RegisterType<NaiveAnomalyDetectionProvider>().As<Atmosphere.Contracts.IAnomalyDetectionProvider>().SingleInstance();
            builder.RegisterType<ElasticEmotionRespository>().As<Atmosphere.Contracts.IEmotionRespository>().SingleInstance();
            builder.RegisterType<AzureEmotionService>().As<Atmosphere.Contracts.IEmotionService>().InstancePerLifetimeScope();
            builder.RegisterType<EnvironmentConfig>().As<Atmosphere.Contracts.IEnvironmentConfig>().SingleInstance();
            builder.RegisterType<FramesBlobStorage>().As<Atmosphere.Contracts.IFrameRepository>().SingleInstance();
            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            // Add our Config object so it can be injected
            services.Configure<EnvironmentConfig>(Configuration);

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
