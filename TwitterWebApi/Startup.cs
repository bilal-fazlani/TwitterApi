using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using TwitterWebApi.AutomapperProfiles;
using TwitterWebApi.ExternalServices.Handle;
using TwitterWebApi.ExternalServices.TwitterSearch;
using TwitterWebApi.Services;

namespace TwitterWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables("twitter_api_");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters(j =>
                {
                    j.Converters.Add(new StringEnumConverter(true));
                });
            services.AddCors();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<TweetDivider>();
            services.AddSingleton<TwitterComponentManager>();
            services.AddSingleton<TweetProfile>();

            if (Configuration["mode"] == "online")
            {
                services.AddSingleton<IHandleService, HandleService>();
                services.AddSingleton<ITwitterSearchService, TwitterSearchService>();
            }
            else
            {
                services.AddSingleton<IHandleService, InMemoryHandleService>();
                services.AddSingleton<ITwitterSearchService, InMemoryTwitterSearchService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            Mapper.Initialize(c => c.AddProfile(serviceProvider.GetService<TweetProfile>()));

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder => builder.WithOrigins("http://localhost:8000").AllowAnyHeader().AllowAnyMethod());

            app.UseMvcWithDefaultRoute();
        }
    }
}