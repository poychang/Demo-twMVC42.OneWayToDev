using ConsoleApp.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    /// <summary>
    /// 啟動應用
    /// </summary>
    public class Startup : IHostedService
    {
        private readonly ILogger<Startup> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly SampleJob _job;

        public Startup(ILogger<Startup> logger, IHostApplicationLifetime appLifetime, SampleJob job)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _job = job;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(SampleJob)} starting.");
                _job.Start();
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
            _logger.LogInformation($"{nameof(SampleJob)} stopping.");
            _job.Stop();

            return Task.CompletedTask;
        }
    }
}
