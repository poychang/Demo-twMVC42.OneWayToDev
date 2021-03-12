using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using WinServiceApp.Models;
using WinServiceApp.WinServices;

namespace WinServiceApp
{
    /// <summary>
    /// 啟動應用
    /// </summary>
    public class Startup : IHostedService
    {
        private readonly ILogger<Startup> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly AppSettings _appSettings;
        private readonly SampleWinService _winService;

        public Startup(ILogger<Startup> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings, SampleWinService winService)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _appSettings = appSettings.Value;
            _winService = winService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(SampleWinService)} starting.");

                switch (ActivateTopshelf())
                {
                    case TopshelfExitCode.Ok:
                        Console.WriteLine($"{_appSettings.ServiceName} status: Ok");
                        break;
                    case TopshelfExitCode.ServiceAlreadyInstalled:
                        Console.WriteLine($"{_appSettings.ServiceName} status: ServiceAlreadyInstalled");
                        break;
                    case TopshelfExitCode.ServiceAlreadyRunning:
                        Console.WriteLine($"{_appSettings.ServiceName} status: ServiceAlreadyRunning");
                        break;
                    case TopshelfExitCode.ServiceNotInstalled:
                        Console.WriteLine($"{_appSettings.ServiceName} status: ServiceNotInstalled");
                        throw new Exception($"{_appSettings.ServiceName} status: ServiceNotInstalled");
                    case TopshelfExitCode.ServiceNotRunning:
                        Console.WriteLine($"{_appSettings.ServiceName} status: ServiceNotRunning");
                        throw new Exception($"{_appSettings.ServiceName} status: ServiceNotRunning");
                    case TopshelfExitCode.ServiceControlRequestFailed:
                        Console.WriteLine($"{_appSettings.ServiceName} status: ServiceControlRequestFailed");
                        throw new Exception($"{_appSettings.ServiceName} status: ServiceControlRequestFailed");
                    case TopshelfExitCode.AbnormalExit:
                        Console.WriteLine($"{_appSettings.ServiceName} status: AbnormalExit");
                        throw new Exception($"{_appSettings.ServiceName} status: AbnormalExit");
                    case TopshelfExitCode.SudoRequired:
                        Console.WriteLine($"{_appSettings.ServiceName} status: SudoRequired");
                        throw new Exception($"{_appSettings.ServiceName} status: SudoRequired");
                    case TopshelfExitCode.NotRunningOnWindows:
                        Console.WriteLine($"{_appSettings.ServiceName} status: NotRunningOnWindows");
                        throw new Exception($"{_appSettings.ServiceName} status: NotRunningOnWindows");
                    default:
                        Console.WriteLine($"{_appSettings.ServiceName} status: Unsupported status...");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception error occurred.");
            }
            finally
            {
                _appLifetime.StopApplication();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(SampleWinService)} stopping.");
            _winService.Stop();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 使用 Topshelf 建立並啟動 Windows Service
        /// </summary>
        /// <returns></returns>
        private TopshelfExitCode ActivateTopshelf() =>
            HostFactory.Run(configurator =>
            {
                var env = string.Empty;

                // 設定執行時所傳入的啟動參數
                configurator.AddCommandLineDefinition(nameof(env), value => { env = value; });
                configurator.ApplyCommandLine();

                // 設定啟動的主要邏輯程式
                configurator.Service<SampleWinService>(settings =>
                {
                    settings.ConstructUsing(() => _winService);
                    settings.BeforeStartingService(context => { });
                    settings.WhenStarted(winService => winService.Start());
                    settings.BeforeStoppingService(context => { context.Stop(); });
                    settings.WhenStopped(winService => { winService.Stop(); });
                });

                // 設定啟動 Windows Service 的身分
                configurator.RunAsLocalSystem()
                    .StartAutomaticallyDelayed()
                    .EnableServiceRecovery(rc => rc.RestartService(5));

                // 設定服務名稱及描述
                configurator.SetServiceName($"{_appSettings.ServiceName}");
                configurator.SetDisplayName($"{_appSettings.ServiceName}");
                configurator.SetDescription($"{_appSettings.ServiceName}");

                // 設定發生例外時的處理方式
                configurator.OnException((exception) =>
                {
                    Console.WriteLine($"{exception.Message}\r\n{exception.Source}\r\n{exception.StackTrace}");
                    Console.WriteLine($"{exception.InnerException?.Message}\r\n{exception.InnerException?.Source}\r\n{exception.InnerException?.StackTrace}");
                });

                // 安裝之後將啟動時所需要的引數寫入 Windows 註冊表中，讓下次啟動時傳遞同樣的引數
                configurator.AfterInstall(installHostSettings =>
                {
                    using (var system = Registry.LocalMachine.OpenSubKey("System"))
                    using (var currentControlSet = system.OpenSubKey("CurrentControlSet"))
                    using (var services = currentControlSet.OpenSubKey("Services"))
                    using (var service = services.OpenSubKey(installHostSettings.ServiceName, true))
                    {
                        const string REG_KEY_IMAGE_PATH = "ImagePath";
                        var imagePath = service?.GetValue(REG_KEY_IMAGE_PATH);
                        service?.SetValue(REG_KEY_IMAGE_PATH, $"{imagePath} -env:{env}");
                    }
                });
            });
    }
}
