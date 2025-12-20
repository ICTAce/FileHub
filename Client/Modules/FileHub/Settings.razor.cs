// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Settings : ModuleBase
{
    private const string ResourceType = "ICTAce.FileHub.Settings, ICTAce.FileHub.Client.Oqtane";

    public override string Title => "FileHub Settings";

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js"),
        new Script("_content/Radzen.Blazor/Radzen.Blazor.js"),
    ];
}
