using JobTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.Background;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<ReminderBackgroundService> _logger;
    private readonly TimeSpan _interval;

    public ReminderBackgroundService(
        IServiceScopeFactory scopes,
        ILogger<ReminderBackgroundService> logger,
        IConfiguration config)
    {
        _scopes = scopes;
        _logger = logger;

        var minutes = int.TryParse(config["Reminders:IntervalMinutes"], out var m) && m > 0 ? m : 60;
        _interval = TimeSpan.FromMinutes(minutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder service started. Checking every {Interval}.", _interval);

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);   // let the app finish starting

        using var timer = new PeriodicTimer(_interval);
        do
        {
            try
            {
                // A background service lives forever, but DbContext is scoped,
                // so each run creates its own scope.
                using var scope = _scopes.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IReminderService>();

                var result = await service.SendDueRemindersAsync();
                _logger.LogInformation("Reminder run finished. Sent: {Sent}, Failed: {Failed}",
                    result.Sent, result.Failed);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Reminder run failed.");   // never let the loop die
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
