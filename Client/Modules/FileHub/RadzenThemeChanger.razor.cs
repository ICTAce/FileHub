// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class RadzenThemeChanger
{
    [Inject]
    protected ISettingService SettingService { get; set; } = default!;

    [Inject]
    private Radzen.ThemeService ThemeService { get; set; } = default!;

    private string RadzenTheme { get; set; } = "default";

    // Preview component state
    private bool _switchValue = true;
    private int _radioValue = 1;
    private int _sliderValue = 50;
    private int _stepsIndex = 1;
    private int _ratingValue = 4;
    private double _gaugeValue = 72;
    private bool _toggleValue = true;

    // Sample data for DataGrid
    private List<SampleData> _sampleData =
    [
        new(1, "Product A", "Electronics", 299.99m, DateTime.Now.AddDays(-5), true),
        new(2, "Product B", "Clothing", 49.99m, DateTime.Now.AddDays(-3), true),
        new(3, "Product C", "Electronics", 199.99m, DateTime.Now.AddDays(-1), false),
        new(4, "Product D", "Home", 89.99m, DateTime.Now.AddDays(-7), true),
        new(5, "Product E", "Clothing", 29.99m, DateTime.Now.AddDays(-2), true),
        new(6, "Product F", "Home", 149.99m, DateTime.Now.AddDays(-10), false),
        new(7, "Product G", "Electronics", 599.99m, DateTime.Now.AddDays(-4), true),
        new(8, "Product H", "Clothing", 79.99m, DateTime.Now.AddDays(-6), true),
    ];

    private record SampleData(int Id, string Name, string Category, decimal Price, DateTime Date, bool IsActive);

    private record ThemeOption(
        string Name,
        string Value,
        string Background,
        string Card,
        string Primary,
        string Accent,
        string Text,
        string ChartColor1,
        string ChartColor2,
        string ChartColor3,
        string BorderRadius);

    private List<ThemeOption> AvailableThemes { get; } =
    [
        new("Default", "default", "#f5f5f5", "#ffffff", "#007bff", "#dc3545", "#212529", "#3700b3", "#ba68c8", "#f06292", "4"),
        new("Dark", "dark", "#121212", "#1e1e1e", "#90caf9", "#f48fb1", "#e0e0e0", "#3700b3", "#ba68c8", "#f06292", "4"),
        new("Material", "material", "#f5f5f5", "#ffffff", "#4340d2", "#e31c65", "#212121", "#3700b3", "#ba68c8", "#f06292", "4"),
        new("Material Dark", "material-dark", "#121212", "#1e1e1e", "#90caf9", "#f48fb1", "#e0e0e0", "#3700b3", "#ba68c8", "#f06292", "4"),
        new("Standard", "standard", "#f4f5f9", "#ffffff", "#1776d1", "#c3002f", "#212121", "#3d7cf4", "#64dfdf", "#f68769", "4"),
        new("Standard Dark", "standard-dark", "#19191a", "#242527", "#90caf9", "#f48fb1", "#e0e0e0", "#3d7cf4", "#64dfdf", "#f68769", "4"),
        new("Humanistic", "humanistic", "#f6f7fa", "#ffffff", "#376df5", "#e91e63", "#212121", "#376df5", "#64dfdf", "#f68769", "4"),
        new("Humanistic Dark", "humanistic-dark", "#28363c", "#38474e", "#90caf9", "#f48fb1", "#e0e0e0", "#376df5", "#64dfdf", "#f68769", "4"),
        new("Software", "software", "#f6f7fa", "#ffffff", "#4a5568", "#ed8936", "#2d3748", "#376df5", "#64dfdf", "#f68769", "4"),
        new("Software Dark", "software-dark", "#28363c", "#3a474d", "#63b3ed", "#f6ad55", "#e2e8f0", "#376df5", "#64dfdf", "#f68769", "4"),
    ];

    private Dictionary<string, string> _settings = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (PageState.User != null)
        {
            _settings = await SettingService.GetUserSettingsAsync(PageState.User.UserId);
            RadzenTheme = SettingService.GetSetting(_settings, "RadzenTheme", "default");
            ThemeService.SetTheme(RadzenTheme);
        }
    }

    private async Task OnThemeSelectedAsync(string theme)
    {
        RadzenTheme = theme;

        // Apply theme immediately
        ThemeService.SetTheme(theme);

        // Save to user settings
        if (PageState.User != null)
        {
            SettingService.SetSetting(_settings, "RadzenTheme", theme);
            await SettingService.UpdateUserSettingsAsync(_settings, PageState.User.UserId);
        }
    }
}
