// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Settings : ModuleBase
{
    [Inject]
    protected ISettingService SettingService { get; set; } = default!;

    [Inject]
    protected IStringLocalizer<Settings> Localizer { get; set; } = default!;

    private const string ResourceType = "ICTAce.FileHub.Settings, ICTAce.FileHub.Client.Oqtane";

    public override string Title => "FileHub Settings";

    private string _value = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _value = SettingService.GetSetting(settings, "SettingName", string.Empty);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public async Task UpdateSettings()
    {
        try
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            SettingService.SetSetting(settings, "SettingName", _value);
            await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);

            AddModuleMessage("Settings updated successfully", MessageType.Success);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task HandleErrorAsync(Exception ex)
    {
        AddModuleMessage(ex.Message, MessageType.Error);
        await Task.CompletedTask;
    }
}
