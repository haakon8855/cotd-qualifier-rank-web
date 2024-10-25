using CotdQualifierRank.Web.Controllers;
using CotdQualifierRank.Web.Services;

namespace CotdQualifierRank.Web.Utils;

public class LeaderboardQueueWorker(IServiceProvider services) : IHostedService, IDisposable
{
    private Timer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("LeaderboardQueueWorker running.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = services.CreateScope();
        var nadeoApiController = scope.ServiceProvider.GetRequiredService<NadeoApiController>();
        var competitionService = scope.ServiceProvider.GetRequiredService<CompetitionService>();
        var nadeoCompetitionService = scope.ServiceProvider.GetRequiredService<NadeoCompetitionService>();

        var queueService = new QueueService(nadeoApiController, nadeoCompetitionService, competitionService);
        await queueService.ProcessMapsAsync();
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("LeaderboardQueueWorker stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}