namespace KHConfAutomation.Interfaces
{
    public interface ISettingsService
    {
        string GetValueOrDefault(string keyName, string defaultValue);
    }
}
