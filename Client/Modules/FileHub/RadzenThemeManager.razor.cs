// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class RadzenThemeManager
{
    [Inject]
    protected ISettingService SettingService { get; set; } = default!;

    [Inject]
    private Radzen.ThemeService ThemeService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (PageState.User != null)
        {
            var settings = await SettingService.GetUserSettingsAsync(PageState.User.UserId);
            var radzenTheme = SettingService.GetSetting(settings, "RadzenTheme", "default");
            ThemeService.SetTheme(radzenTheme);
        }
    }
}
