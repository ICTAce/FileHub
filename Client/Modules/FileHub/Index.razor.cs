// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Index
{
    [Inject] protected ISampleModuleService FileHubService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js"),
    ];

    private List<ListSampleModuleDto>? _filehubs;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var pagedResult = await FileHubService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _filehubs = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading FileHub {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Delete(ListSampleModuleDto filehub)
    {
        try
        {
            await FileHubService.DeleteAsync(filehub.Id, ModuleState.ModuleId).ConfigureAwait(true);
            await logger.LogInformation("FileHub Deleted {Id}", filehub.Id).ConfigureAwait(true);

            var pagedResult = await FileHubService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _filehubs = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting FileHub {Id} {Error}", filehub.Id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
        }
    }
}
