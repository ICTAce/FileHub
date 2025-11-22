// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.ApiTest;

public partial class Index : ModuleBase
{
    [Inject]
    private IMyModuleService MyModuleService { get; set; } = default!;

    private string _selectedEndpoint = "List";
    private int _moduleId;
    private int _id = 1;
    private string _name = string.Empty;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    private bool _loading;
    private string _error = string.Empty;
    private string _responseStatus = string.Empty;
    private string _responseBody = string.Empty;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private int CurrentModuleId => _moduleId != 0 ? _moduleId : ModuleState.ModuleId;

    private async Task ExecuteEndpoint()
    {
        _error = string.Empty;
        _responseStatus = string.Empty;
        _responseBody = string.Empty;

        try
        {
            _loading = true;

            object? result = _selectedEndpoint switch
            {
                "List" => await MyModuleService.ListAsync(CurrentModuleId, _pageNumber, _pageSize),
                "Get" => await MyModuleService.GetAsync(_id, CurrentModuleId),
                "Create" => await MyModuleService.CreateAsync(CurrentModuleId, new CreateUpdateMyModuleDto { Name = _name }),
                "Update" => await MyModuleService.UpdateAsync(_id, CurrentModuleId, new CreateUpdateMyModuleDto { Name = _name }),
                "Delete" => await ExecuteDelete(),
                _ => null
            };

            _responseStatus = $"Success - {_selectedEndpoint} operation completed";
            _responseBody = result is not null
                ? JsonSerializer.Serialize(result, _jsonOptions)
                : "Operation completed successfully";
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            _responseStatus = "Error";
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task<object?> ExecuteDelete()
    {
        await MyModuleService.DeleteAsync(_id, CurrentModuleId);
        return null;
    }
}
