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

    private List<ListMyModulesResponse>? _MyModules;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = new ListMyModulesRequest { ModuleId = ModuleState.ModuleId };
            var pagedResult = await MyModuleService.ListAsync(request);
            _MyModules = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading MyModule {Error}", ex.Message);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Delete(ListMyModulesResponse myModule)
    {
        try
        {
            var request = new DeleteMyModuleRequest 
            { 
                Id = myModule.Id, 
                ModuleId = ModuleState.ModuleId 
            };
            await MyModuleService.DeleteAsync(request);
            await logger.LogInformation("MyModule Deleted {Id}", myModule.Id);
            
            var listRequest = new ListMyModulesRequest { ModuleId = ModuleState.ModuleId };
            var pagedResult = await MyModuleService.ListAsync(listRequest);
            _MyModules = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting MyModule {Id} {Error}", myModule.Id, ex.Message);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
        }
    }
}
