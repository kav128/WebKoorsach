using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using EduJournal.BLL;
using EduJournal.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;

namespace EduJournal.Presentation.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddFluentValidation(fvc =>
                {
                    fvc.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                    fvc.LocalizationEnabled = false;
                });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduJournal.Presentation.Web", Version = "v1" });
            });
            
            
            if (CurrentEnvironment.IsEnvironment("Testing"))
                services.AddTestingDataAccessLayer();
            else
            {
                string? connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING");
                services.AddDataAccessLayer(connectionString, new NLogLoggerFactory());
            }

            services.AddBusinessLogicLayer();

            services.AddAutoMapper(config => config.AddProfiles(BusinessLogicLayerDI.GetAutoMapperProfiles()
                .Concat(new[] { new ModelProfile() })));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduJournal.Presentation.Web v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseMiddleware<ErrorHandlerMiddleware>();
            
            //app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
