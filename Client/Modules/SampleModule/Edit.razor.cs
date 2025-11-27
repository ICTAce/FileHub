// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.SampleModule;

public partial class Edit
{
    [Inject] protected ISampleModuleService SampleModuleService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Edit> Localizer { get; set; } = default!;

    public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

    public override string Actions => "Add,Edit";

    public override string Title => "Manage SampleModule";

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
    private string _cancelUrl = string.Empty;
    private bool _showAuditInfo;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            try
            {
                _cancelUrl = NavigateUrl();
            }
            catch
            {
                _cancelUrl = "#";
            }

            _showAuditInfo = string.Equals(PageState.Action, "Edit", StringComparison.Ordinal);

            if (_showAuditInfo)
            {
                _id = Int32.Parse(PageState.QueryString["id"], System.Globalization.CultureInfo.InvariantCulture);
                var sampleModule = await SampleModuleService.GetAsync(_id, ModuleState.ModuleId).ConfigureAwait(true);
                if (sampleModule != null)
                {
                    _name = sampleModule.Name;
                    _createdby = sampleModule.CreatedBy;
                    _createdon = sampleModule.CreatedOn;
                    _modifiedby = sampleModule.ModifiedBy;
                    _modifiedon = sampleModule.ModifiedOn;
                }
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading SampleModule {Id} {Error}", _id, ex.Message).ConfigureAwait(true);
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
                    var id = await SampleModuleService.CreateAsync(ModuleState.ModuleId, dto).ConfigureAwait(true);

                    await logger.LogInformation("SampleModule Created {Id}", id).ConfigureAwait(true);
                }
                else
                {
                    var dto = new CreateAndUpdateSampleModuleDto
                    {
                        Name = _name
                    };
                    var id = await SampleModuleService.UpdateAsync(_id, ModuleState.ModuleId, dto).ConfigureAwait(true);

                    await logger.LogInformation("SampleModule Updated {Id}", id).ConfigureAwait(true);
                }

                try
                {
                    NavigationManager.NavigateTo(NavigateUrl());
                }
                catch
                {
                    // Navigation may fail in test environments
                }
            }
            else
            {
                AddModuleMessage(Localizer["Message.SaveValidation"], MessageType.Warning);
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Saving SampleModule {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.SaveError"], MessageType.Error);
        }
    }
}
