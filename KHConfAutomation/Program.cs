using KHConfAutomation.Interfaces;
using KHConfAutomation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KHConfAutomation;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<IPlaywrightService, PlaywrightService>();
                services.AddSingleton<IMeetingService, MeetingService>();
                services.AddSingleton<IShutdownService, ShutdownService>();
                services.AddSingleton<ISettingsService, SettingService>();
            });
}