namespace KHConfAutomation.Interfaces;

public interface IPlaywrightService
{
    Task<bool> StartMeeting();
    Task<bool> CloseMeeting();
}
