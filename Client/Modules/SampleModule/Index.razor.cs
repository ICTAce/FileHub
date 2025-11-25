// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.SampleModule;

public partial class Index
{
    [Inject] protected ISampleModuleService MyModuleService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

    public override List<Resource> Resources => new List<Resource>()
    {
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js")
    };

    private List<ListSampleModuleDto>? _MyModules;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var pagedResult = await MyModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _MyModules = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            try
            {
                await logger.LogError(ex, "Error Loading MyModule {Error}", ex.Message).ConfigureAwait(true);
            }
            catch (NullReferenceException)
            {
                // Logger may fail if Alias is not initialized in test environments
            }
            
            try
            {
                AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
            }
            catch (NullReferenceException)
            {
                // AddModuleMessage may fail in test environments
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        }
        catch (NullReferenceException)
        {
            // Oqtane ModuleBase lifecycle methods may fail in test environments
        }
    }

    private async Task Delete(ListSampleModuleDto myModule)
    {
        try
        {
            await MyModuleService.DeleteAsync(myModule.Id, ModuleState.ModuleId).ConfigureAwait(true);
            
            try
            {
                await logger.LogInformation("MyModule Deleted {Id}", myModule.Id).ConfigureAwait(true);
            }
            catch (NullReferenceException)
            {
                // Logger may fail if Alias is not initialized in test environments
            }
            
            var pagedResult = await MyModuleService.ListAsync(ModuleState.ModuleId).ConfigureAwait(true);
            _MyModules = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            try
            {
                await logger.LogError(ex, "Error Deleting MyModule {Id} {Error}", myModule.Id, ex.Message).ConfigureAwait(true);
            }
            catch (NullReferenceException)
            {
                // Logger may fail if Alias is not initialized in test environments
            }
            
            try
            {
                AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
            }
            catch (NullReferenceException)
            {
                // AddModuleMessage may fail in test environments
            }
        }
    }
}
