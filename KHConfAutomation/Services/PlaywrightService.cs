using KHConfAutomation.Interfaces;
using Microsoft.Playwright;

namespace KHConfAutomation.Services;

public class PlaywrightService : IPlaywrightService
{
    private IPlaywright? _playwright = null;
    private readonly ISettingsService _settingsService;

    private readonly string _meetingUrl;
    private readonly string _inputName;
    private readonly string _browserState;

    public PlaywrightService(ISettingsService settingService)
    {
        _settingsService = settingService;

        _meetingUrl = _settingsService.GetValueOrDefault("MeetingUrl", "https://meet.khconf.com/187016061");

        _inputName = _settingsService.GetValueOrDefault("InputName", "Joaquim Vieira");

        _browserState = _settingsService.GetValueOrDefault("BrowserState", "browser_state");
    }

    public async Task<bool> StartMeeting()
    {
        try
        {
            var browser = await LaunchPlaywright();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(_meetingUrl);

            await Task.Delay(1000);

            if (await AreTermsVisible(page))
            {
                await AcceptTerms(page);
            }

            await Task.Delay(1000);

            if (await EnterName(page) == false)
            {
                await browser.CloseAsync();
                await CloseMeeting();
                return false;
            }

            await Task.Delay(1000);

            if (await JoinMeeting(page) == false)
            {
                await browser.CloseAsync();
                await CloseMeeting();
                return false;
            }

            await Task.Delay(1000);

            if (await WaitingForTheHost(page))
            {
                Console.WriteLine("Waiting for the host to enter");
                await browser.CloseAsync();
                await CloseMeeting();
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Playwright: {ex.Message}");
            return false;
        }
    }

    private async Task<IBrowserContext> LaunchPlaywright()
    {
        _playwright = await Playwright.CreateAsync();

        return await _playwright.Chromium.LaunchPersistentContextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _browserState), new()
        {
            Headless = false,
            ExecutablePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
            Permissions = ["camera", "microphone"],
            Args = ["--start-maximized"],
            ViewportSize = ViewportSize.NoViewport
        });
    }

    private async Task<bool> AreTermsVisible(IPage page)
    {
        return await page.GetByText("Terms of Use & Privacy Notice").IsVisibleAsync();
    }

    private async Task AcceptTerms(IPage page)
    {
        await page.EvaluateAsync($@"
            $(""div.css-rhnteu-content"").attr(""id"",""termsDiv"");
            $(""#termsDiv"").scrollTop($(""#termsDiv"")[0].scrollHeight);
        ");

        await Task.Delay(1000);

        await page.GetByTitle("I Accept").ClickAsync();
    }

    private async Task<bool> EnterName(IPage page)
    {
        var isInputVisible = await page.GetByPlaceholder("Enter your name").IsVisibleAsync();
        if (isInputVisible == false) return false;

        var inputElement = page.GetByPlaceholder("Enter your name");
        await inputElement.FillAsync(_inputName);

        Console.WriteLine("Filled in the name");
        return true;
    }

    private async Task<bool> JoinMeeting(IPage page)
    {
        var labelToClick = page.GetByLabel("Join meeting");
        await labelToClick.ClickAsync();

        Console.WriteLine("Trying to join meeting");
        return true;
    }

    private async Task<bool> WaitingForTheHost(IPage page)
    {
        var doesLabelHostExist = await page.GetByLabel("I am the host").IsVisibleAsync();
        if (doesLabelHostExist == true) return true;

        return false;
    }

    public async Task<bool> CloseMeeting()
    {
        if (_playwright == null) return true;

        _playwright.Dispose();

        return true;
    }
}
