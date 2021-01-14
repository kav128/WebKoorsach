using System;
using System.Threading.Tasks;
using EduJournal.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Polly;
using Polly.Retry;
using LogLevel = NLog.LogLevel;

namespace EduJournal.Presentation.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();  
            try  
            {  
                logger.Debug("init main function");  
                IHost host = CreateHostBuilder(args).UseNLog().Build();
                ApplicationContext? dbContext = host.Services.GetService<IDbContextFactory<ApplicationContext>>()?.CreateDbContext();
                AsyncRetryPolicy retryForeverAsync = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3), (exception, i, ts) => logger.Log(LogLevel.Warn, exception,
                        $"Unable to migrate database. Attempt {i}. Retrying in {ts.TotalSeconds} s."));
                await retryForeverAsync.ExecuteAsync(() => dbContext?.Database.MigrateAsync());
                await host.RunAsync();
            }  
            catch (Exception ex)  
            {  
                logger.Error(ex, "Exiting with error");  
                throw;  
            }  
            finally  
            {  
                LogManager.Shutdown();  
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
