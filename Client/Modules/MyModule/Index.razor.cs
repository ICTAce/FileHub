// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.MyModule;

public partial class Index
{
    [Inject] protected IMyModuleService MyModuleService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

    public override List<Resource> Resources => new List<Resource>()
    {
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js")
    };

    private List<ListMyModuleDto>? _MyModules;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var pagedResult = await MyModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _MyModules = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading MyModule {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Delete(ListMyModuleDto myModule)
    {
        try
        {
            await MyModuleService.DeleteAsync(myModule.Id, ModuleState.ModuleId).ConfigureAwait(true);
            await logger.LogInformation("MyModule Deleted {Id}", myModule.Id).ConfigureAwait(true);
            
            var pagedResult = await MyModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _MyModules = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting MyModule {Id} {Error}", myModule.Id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
        }
    }
}
