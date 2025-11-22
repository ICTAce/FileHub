// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.ApiTest;

public partial class Index : ModuleBase
{
    [Inject]
    private IMyModuleService MyModuleService { get; set; } = default!;

    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    private string _selectedService = "MyModule";
    private string _selectedEndpoint = "List";
    private int _moduleId;
    private int _id = 1;
    private string _name = string.Empty;
    private int _viewOrder;
    private int _parentId;
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

            object? result = _selectedService switch
            {
                "MyModule" => await ExecuteMyModuleEndpoint(),
                "Category" => await ExecuteCategoryEndpoint(),
                _ => null
            };

            _responseStatus = $"Success - {_selectedService}.{_selectedEndpoint} operation completed";
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

    private Task<object?> ExecuteMyModuleEndpoint()
    {
        return _selectedEndpoint switch
        {
            "List" => MyModuleService.ListAsync(CurrentModuleId, _pageNumber, _pageSize).ContinueWith(t => (object?)t.Result),
            "Get" => MyModuleService.GetAsync(_id, CurrentModuleId).ContinueWith(t => (object?)t.Result),
            "Create" => MyModuleService.CreateAsync(CurrentModuleId, new CreateUpdateMyModuleDto { Name = _name }).ContinueWith(t => (object?)t.Result),
            "Update" => MyModuleService.UpdateAsync(_id, CurrentModuleId, new CreateUpdateMyModuleDto { Name = _name }).ContinueWith(t => (object?)t.Result),
            "Delete" => ExecuteDeleteMyModule(),
            _ => Task.FromResult<object?>(null)
        };
    }

    private Task<object?> ExecuteCategoryEndpoint()
    {
        return _selectedEndpoint switch
        {
            "List" => CategoryService.ListAsync(CurrentModuleId, _pageNumber, _pageSize).ContinueWith(t => (object?)t.Result),
            "Get" => CategoryService.GetAsync(_id, CurrentModuleId).ContinueWith(t => (object?)t.Result),
            "Create" => CategoryService.CreateAsync(CurrentModuleId, new CreateUpdateCategoryDto 
            { 
                Name = _name, 
                ViewOrder = _viewOrder, 
                ParentId = _parentId 
            }).ContinueWith(t => (object?)t.Result),
            "Update" => CategoryService.UpdateAsync(_id, CurrentModuleId, new CreateUpdateCategoryDto 
            { 
                Name = _name, 
                ViewOrder = _viewOrder, 
                ParentId = _parentId 
            }).ContinueWith(t => (object?)t.Result),
            "Delete" => ExecuteDeleteCategory(),
            _ => Task.FromResult<object?>(null)
        };
    }

    private async Task<object?> ExecuteDeleteMyModule()
    {
        await MyModuleService.DeleteAsync(_id, CurrentModuleId);
        return null;
    }

    private async Task<object?> ExecuteDeleteCategory()
    {
        await CategoryService.DeleteAsync(_id, CurrentModuleId);
        return null;
    }
}
