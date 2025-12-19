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

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js"),
        new Script("_content/Radzen.Blazor/Radzen.Blazor.js"),
    ];

    private async Task HandleErrorAsync(Exception ex)
    {
        AddModuleMessage(ex.Message, MessageType.Error);
        await Task.CompletedTask;
    }
}
