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

    public override List<Resource> Resources => new()
    {
        new Script($"_content/Radzen.Blazor/Radzen.Blazor.js?v={typeof(Radzen.Colors).Assembly.GetName().Version}")
    };

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            // Request all categories (use a large page size to get all items)
            Categories = await CategoryService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue);
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

        // Clear all children lists before rebuilding the tree
        foreach (var category in Categories.Items)
        {
            category.Children.Clear();
        }

        var categoryDict = Categories.Items.ToDictionary(c => c.Id, c => c);

        // Get root items (ParentId = 0 or parent doesn't exist in the dataset)
        TreeData = Categories.Items
            .Where(c => c.ParentId == 0 || !categoryDict.ContainsKey(c.ParentId))
            .OrderBy(c => c.ViewOrder)
            .ThenBy(c => c.Name)
            .ToList();

        // Build parent-child relationships
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
