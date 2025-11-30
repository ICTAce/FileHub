// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Category : ModuleBase
{
    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    protected PagedResult<ListCategoryDto> Categories { get; set; } = new();
    protected List<ListCategoryDto> TreeData { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Categories = await CategoryService.ListAsync(ModuleState.ModuleId);
            TreeData = Categories.Items?.ToList() ?? new List<ListCategoryDto>();
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

    protected void OnAddCategory(ListCategoryDto category)
    {
        // Handle add subcategory
    }

    protected void OnEditCategory(ListCategoryDto category)
    {
        // Handle edit category
    }

    protected void OnDeleteCategory(ListCategoryDto category)
    {
        // Handle delete category
    }
}
