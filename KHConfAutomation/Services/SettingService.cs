using KHConfAutomation.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KHConfAutomation.Services
{
    public class SettingService : ISettingsService
    {
        private readonly IConfiguration _configuration;
        public SettingService()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appSettings.json", optional: false);

            _configuration = builder.Build();
        }

        public string GetValueOrDefault(string keyName, string defaultValue)
        {
            var value = _configuration[keyName];
            if (value == null) return defaultValue;

            return value;
        }
    }
}
