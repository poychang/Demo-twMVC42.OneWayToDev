using CommonUtility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text.Json;
using WinServiceApp.Models;
using WinServiceApp.WinServices;

namespace WinServiceApp
{
    public static class Program
    {
        /// <summary>
        /// 主程式進入點
        /// </summary>
        /// <remarks>
        /// 指定環境變數，請使用以下方式傳入 -env:Debug
        /// </remarks>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunAsync();
        }

        /// <summary>
        /// 建立泛型主機，並處理相關設定值與 DI 服務
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration((config) =>
                {
                    var env = args.FirstOrDefault(arg => arg.StartsWith("-env:"))?.Split(':')[1];
                    Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", env);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    ConsoleHelper.WriteLine($"Environment: {hostContext.HostingEnvironment.EnvironmentName}", ConsoleColor.Magenta);
                    config.SetBasePath(AppContext.BaseDirectory);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration.Get<AppSettings>();
                    ConsoleHelper.WriteLine($"Configuration: {JsonSerializer.Serialize(configuration)}", ConsoleColor.Magenta);
                    ConsoleHelper.WriteLine();

                    services.Configure<AppSettings>(hostContext.Configuration);
                    services.AddHostedService<Startup>();
                    services.AddTransient<SampleWinService>();
                });
    }
}
