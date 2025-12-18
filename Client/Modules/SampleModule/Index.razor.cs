// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.SampleModule;

public partial class Index
{
    [Inject] protected ISampleModuleService SampleModuleService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js")
    ];

    private List<ListSampleModuleDto>? _samplesModules;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var pagedResult = await SampleModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _samplesModules = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading SampleModule {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Delete(ListSampleModuleDto sampleModule)
    {
        try
        {
            await SampleModuleService.DeleteAsync(sampleModule.Id, ModuleState.ModuleId).ConfigureAwait(true);
            await logger.LogInformation("SampleModule Deleted {Id}", sampleModule.Id).ConfigureAwait(true);
            var pagedResult = await SampleModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _samplesModules = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting SampleModule {Id} {Error}", sampleModule.Id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
        }
    }
}
