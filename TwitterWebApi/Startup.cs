using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using TwitterWebApi.Models.AutomapperProfiles;
using TwitterWebApi.Services.Handle;
using TwitterWebApi.Services.TwitterSearch;

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

            Mapper.Initialize(mapperConfiguration => mapperConfiguration.AddProfile(new TweetProfile()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder => builder.WithOrigins("http://localhost:8000").AllowAnyHeader().AllowAnyMethod());

            app.UseMvcWithDefaultRoute();
        }
    }
}