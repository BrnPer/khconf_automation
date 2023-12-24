using KHConfAutomation.Interfaces;
using System.Diagnostics;

namespace KHConfAutomation.Services
{
    public class ShutdownService : IShutdownService
    {
        private readonly ISettingsService _settingsService;
        private readonly bool reallyTurnOffPC = false;
        public ShutdownService(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            var tmpValue = _settingsService.GetValueOrDefault("ShutdownPC", "false");
            reallyTurnOffPC = tmpValue == "true" ? true : false;
        }
        public async Task Shutdown()
        {
            Console.WriteLine("Shutting down ");
            if (reallyTurnOffPC == true)
            {
                var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);
            }
        }
    }
}
