namespace KHConfAutomation.Interfaces
{
    public interface IMeetingService
    {
        Task Started();
        Task<bool> ShouldStartMeeting();
        Task<bool> ShouldCloseMeeting();
    }
}
