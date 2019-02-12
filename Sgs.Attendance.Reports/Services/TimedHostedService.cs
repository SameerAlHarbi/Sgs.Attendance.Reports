using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        public IServiceProvider _services { get; }
        private Timer _timer;

        public TimedHostedService(IServiceProvider services, ILogger<TimedHostedService> logger)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        bool locked = false;
        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            if (locked)
            {
                _logger.LogInformation("Timed Background Service is Locked !");
                return;
            }

            locked = true;

            try
            {
                _logger.LogInformation("Timed Background Service is Started.");

                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IScopedProcessingService>();

                   var result = await scopedProcessingService.DoWork();
                    if(result || !result)
                    {
                        locked = false;
                        _logger.LogInformation("Timed Background Service is Finished.");
                    }
                }
            }
            catch (Exception ex)
            {
                locked = false;
                _logger.LogError(ex.Message);
                _logger.LogInformation("Timed Background Service is Finished.");
            }
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            locked = false;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            locked = false;
            _timer?.Dispose();
        }

    }
}
