using Funq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisStackOverflow.ServiceInterface;
using System;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Redis;
using ServiceStack.Text;
using IRepository = RedisStackOverflow.ServiceInterface.IRepository;

namespace RedisStackOverflow
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost());
        }
    }

    /// <summary>
    /// Create your ServiceStack web service application with a singleton AppHost.
    /// </summary>  
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public AppHost() : base("Redis StackOverflow", typeof(QuestionsService).Assembly) 
        { 
            AppSettings = new MultiAppSettings(
                new EnvironmentVariableSettings(),
                new AppSettings());
        }

        /// <summary>
        /// Configure the container with the necessary routes for your ServiceStack application.
        /// </summary>
        /// <param name="container">The built-in IoC used with ServiceStack.</param>
        public override void Configure(Container container)
        {
            //Show StackTrace in Web Service Exceptions
            SetConfig(new HostConfig
            {
                DebugMode = true,
                UseCamelCase = false,
            });

            //Register any dependencies you want injected into your services
            container.Register<IRedisClientsManager>(c =>
                new RedisManagerPool(AppSettings.Get("REDIS_HOST", defaultValue:"localhost")));

            container.Register<IRepository>(c => new Repository(c.Resolve<IRedisClientsManager>()));
        }
    }
}
