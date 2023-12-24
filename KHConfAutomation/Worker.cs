using KHConfAutomation.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KHConfAutomation;
public class Worker : BackgroundService
{
    private readonly IPlaywrightService _playwrightService;
    private readonly IMeetingService _meetingService;
    private readonly IShutdownService _shutdownService;
    private readonly IHostApplicationLifetime _lifeTime;

    private const int TASK_DELAY = 60000;

    public Worker(IPlaywrightService playwrightService, IMeetingService meetingService, IShutdownService shutdownService, IHostApplicationLifetime lifeTime)
    {
        _playwrightService = playwrightService;
        _meetingService = meetingService;
        _shutdownService = shutdownService;
        _lifeTime = lifeTime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            var shouldStartMeeting = await _meetingService.ShouldStartMeeting();
            if (shouldStartMeeting == true)
            {
                Console.WriteLine("Should start meeting");
                var resultStartMeeting = await _playwrightService.StartMeeting();
                if (resultStartMeeting == true)
                {
                    await _meetingService.Started();
                }
            }

            var shouldCloseMeeting = await _meetingService.ShouldCloseMeeting();
            if (shouldCloseMeeting == true)
            {
                Console.WriteLine("Should close meeting");
                var resultCloseMeeting = await _playwrightService.CloseMeeting();
                if (resultCloseMeeting == true)
                {
                    await _shutdownService.Shutdown();
                    _lifeTime.StopApplication();
                }
            }

            if (stoppingToken.IsCancellationRequested == false)
            {
                await Task.Delay(TASK_DELAY, stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Closing everything!");
        await _playwrightService.CloseMeeting();
    }
}