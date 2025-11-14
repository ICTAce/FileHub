// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.MyModule;

public partial class Edit
{
    [Inject] protected IMyModuleService MyModuleService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Edit> Localizer { get; set; } = default!;

    public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

    public override string Actions => "Add,Edit";

    public override string Title => "Manage MyModule";

    public override List<Resource> Resources => new List<Resource>()
    {
        new Stylesheet(ModulePath() + "Module.css")
    };

    private ElementReference form;
    private bool validated = false;

    private int _id;
    private string _name = string.Empty;
    private string _createdby = string.Empty;
    private DateTime _createdon;
    private string _modifiedby = string.Empty;
    private DateTime _modifiedon;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (PageState.Action == "Edit")
            {
                _id = Int32.Parse(PageState.QueryString["id"]);
                var request = new GetMyModuleRequest 
                { 
                    Id = _id, 
                    ModuleId = ModuleState.ModuleId 
                };
                var myModule = await MyModuleService.GetAsync(request);
                if (myModule != null)
                {
                    _name = myModule.Name;
                    _createdby = myModule.CreatedBy;
                    _createdon = myModule.CreatedOn;
                    _modifiedby = myModule.ModifiedBy;
                    _modifiedon = myModule.ModifiedOn;
                }
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading MyModule {Id} {Error}", _id, ex.Message);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Save()
    {
        try
        {
            validated = true;
            var interop = new Oqtane.UI.Interop(JSRuntime);
            if (await interop.FormValid(form))
            {
                if (PageState.Action == "Add")
                {
                    var command = new CreateMyModuleRequest
                    {
                        ModuleId = ModuleState.ModuleId,
                        Name = _name
                    };
                    var id = await MyModuleService.CreateAsync(command);
                    await logger.LogInformation("MyModule Created {Id}", id);
                }
                else
                {
                    var command = new UpdateMyModuleRequest
                    {
                        Id = _id,
                        ModuleId = ModuleState.ModuleId,
                        Name = _name
                    };
                    var id = await MyModuleService.UpdateAsync(command);
                    await logger.LogInformation("MyModule Updated {Id}", id);
                }
                NavigationManager.NavigateTo(NavigateUrl());
            }
            else
            {
                AddModuleMessage(Localizer["Message.SaveValidation"], MessageType.Warning);
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Saving MyModule {Error}", ex.Message);
            AddModuleMessage(Localizer["Message.SaveError"], MessageType.Error);
        }
    }
}
