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

    protected override async Task OnInitializedAsync()
    {
    }

    private async Task HandleErrorAsync(Exception ex)
    {
        AddModuleMessage(ex.Message, MessageType.Error);
        await Task.CompletedTask;
    }
}
