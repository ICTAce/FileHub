// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.MyModule;

public partial class Category : ModuleBase
{
    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    protected PagedResult<ListCategoryDto> Categories { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Categories = await CategoryService.ListAsync(ModuleState.ModuleId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
