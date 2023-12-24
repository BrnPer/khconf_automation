using KHConfAutomation.Interfaces;

namespace KHConfAutomation.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly ISettingsService _settingsService;
        public readonly DayOfWeek _dayOfWeekForMeeting;
        public readonly TimeOnly _hoursToStartMeeting;
        public readonly TimeOnly _hoursToFinishMeeting;

        private bool _alreadyStarted = false;

        public MeetingService(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            var tmpDayOfWeek = _settingsService.GetValueOrDefault("DayWeekOfMeeting", "2");
            _dayOfWeekForMeeting = (DayOfWeek)Enum.GetValues(typeof(DayOfWeek)).GetValue(int.Parse(tmpDayOfWeek))!;

            var tmpMeetingStart = _settingsService.GetValueOrDefault("MeetingStart", "14:25");
            _hoursToStartMeeting = TimeOnly.Parse(tmpMeetingStart);

            var tmpMeetingEnd = _settingsService.GetValueOrDefault("MeetingEnd", "17:15");
            _hoursToFinishMeeting = TimeOnly.Parse(tmpMeetingEnd);
        }

        public async Task Started()
        {
            _alreadyStarted = true;
            Console.WriteLine("Meeting is starting...");
        }

        public async Task<bool> ShouldCloseMeeting()
        {
            if (DateTime.Now.DayOfWeek != _dayOfWeekForMeeting) return false;

            if (TimeOnly.FromDateTime(DateTime.Now) > _hoursToFinishMeeting) return true;

            return false;
        }

        public async Task<bool> ShouldStartMeeting()
        {
            if (_alreadyStarted) return false;

            if (DateTime.Now.DayOfWeek != _dayOfWeekForMeeting) return false;

            if (TimeOnly.FromDateTime(DateTime.Now) < _hoursToStartMeeting) return false;

            if (TimeOnly.FromDateTime(DateTime.Now) > _hoursToFinishMeeting) return false;

            return true;
        }
    }
}
