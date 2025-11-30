// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Category : ModuleBase
{
    private List<ListCategoryDto> TreeData = new();

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
            CreateTreeStructure();
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

    private void CreateTreeStructure()
    {
        if (Categories.Items is null || !Categories.Items.Any())
        {
            TreeData = new();
            return;
        }

        var categoryDict = Categories.Items.ToDictionary(c => c.Id, c => c);

        TreeData = Categories.Items
            .Where(c => c.ParentId == 0 || !categoryDict.ContainsKey(c.ParentId))
            .OrderBy(c => c.ViewOrder)
            .ThenBy(c => c.Name)
            .ToList();

        foreach (var category in Categories.Items)
        {
            if (category.ParentId != 0 && categoryDict.TryGetValue(category.ParentId, out var parent))
            {
                parent.Children.Add(category);
            }
        }

        SortChildren(TreeData);
    }

    private void SortChildren(List<ListCategoryDto> categories)
    {
        foreach (var category in categories)
        {
            if (category.Children.Any())
            {
                category.Children = category.Children
                    .OrderBy(c => c.ViewOrder)
                    .ThenBy(c => c.Name)
                    .ToList();

                SortChildren(category.Children);
            }
        }
    }
}
