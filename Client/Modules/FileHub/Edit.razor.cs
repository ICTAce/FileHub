// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Edit
{
    [Inject] protected ISampleModuleService MyModuleService { get; set; } = default!;
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
    private bool _validated;

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
            if (string.Equals(PageState.Action, "Edit", StringComparison.Ordinal))
            {
                _id = Int32.Parse(PageState.QueryString["id"], System.Globalization.CultureInfo.InvariantCulture);
                var myModule = await MyModuleService.GetAsync(_id, ModuleState.ModuleId).ConfigureAwait(true);
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
            await logger.LogError(ex, "Error Loading MyModule {Id} {Error}", _id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task Save()
    {
        try
        {
            _validated = true;
            var interop = new Oqtane.UI.Interop(JSRuntime);
            if (await interop.FormValid(form))
            {
                if (string.Equals(PageState.Action, "Add", StringComparison.Ordinal))
                {
                    var dto = new CreateAndUpdateSampleModuleDto
                    {
                        Name = _name
                    };
                    var id = await MyModuleService.CreateAsync(ModuleState.ModuleId, dto).ConfigureAwait(true);
                    await logger.LogInformation("MyModule Created {Id}", id).ConfigureAwait(true);
                }
                else
                {
                    var dto = new CreateAndUpdateSampleModuleDto
                    {
                        Name = _name
                    };
                    var id = await MyModuleService.UpdateAsync(_id, ModuleState.ModuleId, dto).ConfigureAwait(true);
                    await logger.LogInformation("MyModule Updated {Id}", id).ConfigureAwait(true);
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
            await logger.LogError(ex, "Error Saving MyModule {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.SaveError"], MessageType.Error);
        }
    }
}
