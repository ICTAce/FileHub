// Licensed to ICTAce under the MIT license.

using Radzen;

namespace ICTAce.FileHub;

public partial class Edit
{
    [Inject] protected Services.IFileService FileService { get; set; } = default!;
    [Inject] protected ICategoryService CategoryService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Edit> Localizer { get; set; } = default!;

    public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

    public override string Actions => "Add,Edit";

    public override string Title => "Manage File";

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script("_content/Radzen.Blazor/Radzen.Blazor.js")
    ];

    private ElementReference form;
    private bool _validated;

    private int _id;
    private string _name = string.Empty;
    private string _fileName = string.Empty;
    private string _imageName = string.Empty;
    private string? _description;
    private string _fileSize = string.Empty;
    private int _downloads;
    
    private string _createdby = string.Empty;
    private DateTime _createdon;
    private string _modifiedby = string.Empty;
    private DateTime _modifiedon;

    private List<ListCategoryDto> _treeData = [];
    private ListCategoryDto _rootNode = new() { Name = "Categories" };
    private IEnumerable<object>? _selectedCategories;
    private bool _isLoadingCategories;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Load categories
            await LoadCategories();

            if (string.Equals(PageState.Action, "Edit", StringComparison.Ordinal))
            {
                _id = Int32.Parse(PageState.QueryString["id"], System.Globalization.CultureInfo.InvariantCulture);
                var file = await FileService.GetAsync(_id, ModuleState.ModuleId).ConfigureAwait(true);
                if (file != null)
                {
                    _name = file.Name;
                    _fileName = file.FileName;
                    _imageName = file.ImageName;
                    _description = file.Description;
                    _fileSize = file.FileSize;
                    _downloads = file.Downloads;
                    _createdby = file.CreatedBy;
                    _createdon = file.CreatedOn;
                    _modifiedby = file.ModifiedBy;
                    _modifiedon = file.ModifiedOn;
                }
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading File {Id} {Error}", _id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private async Task LoadCategories()
    {
        try
        {
            _isLoadingCategories = true;
            var categories = await CategoryService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue);
            CreateTreeStructure(categories);
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading Categories {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage("Failed to load categories", MessageType.Warning);
        }
        finally
        {
            _isLoadingCategories = false;
        }
    }

    private void CreateTreeStructure(PagedResult<ListCategoryDto> categories)
    {
        if (categories.Items is null || !categories.Items.Any())
        {
            _treeData = [];
            _rootNode = new ListCategoryDto
            {
                Id = 0,
                Name = "Categories",
                ParentId = -1,
                ViewOrder = 0,
                IsExpanded = true,
                Children = []
            };
            return;
        }

        foreach (var category in categories.Items)
        {
            category.Children.Clear();
        }

        var categoryDict = categories.Items.ToDictionary(c => c.Id, c => c);

        _treeData = categories.Items
            .Where(c => c.ParentId is null || !categoryDict.ContainsKey(c.ParentId.Value))
            .OrderBy(c => c.ViewOrder)
            .ThenBy(c => c.Name, StringComparer.Ordinal)
            .ToList();

        foreach (var category in categories.Items)
        {
            if (category.ParentId is not null && categoryDict.TryGetValue(category.ParentId.Value, out var parent))
            {
                parent.Children.Add(category);
            }
        }

        SortChildren(_treeData);

        _rootNode = new ListCategoryDto
        {
            Id = 0,
            Name = "Categories",
            ParentId = null,
            ViewOrder = 0,
            IsExpanded = true,
            Children = _treeData
        };
    }

    private static void SortChildren(List<ListCategoryDto> categories)
    {
        foreach (var category in categories)
        {
            if (category.Children.Any())
            {
                category.Children = category.Children
                    .OrderBy(c => c.ViewOrder)
                    .ThenBy(c => c.Name, StringComparer.Ordinal)
                    .ToList();

                SortChildren(category.Children.ToList());
            }
        }
    }

    private void OnCategorySelectionChanged(TreeEventArgs args)
    {
        _selectedCategories = args.Value as IEnumerable<object>;
        StateHasChanged();
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
                    var dto = new CreateAndUpdateFileDto
                    {
                        Name = _name,
                        FileName = _fileName,
                        ImageName = _imageName,
                        Description = _description,
                        FileSize = _fileSize,
                        Downloads = _downloads
                    };
                    var id = await FileService.CreateAsync(ModuleState.ModuleId, dto).ConfigureAwait(true);
                    await logger.LogInformation("File Created {Id}", id).ConfigureAwait(true);
                }
                else
                {
                    var dto = new CreateAndUpdateFileDto
                    {
                        Name = _name,
                        FileName = _fileName,
                        ImageName = _imageName,
                        Description = _description,
                        FileSize = _fileSize,
                        Downloads = _downloads
                    };
                    var id = await FileService.UpdateAsync(_id, ModuleState.ModuleId, dto).ConfigureAwait(true);
                    await logger.LogInformation("File Updated {Id}", id).ConfigureAwait(true);
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
            await logger.LogError(ex, "Error Saving File {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.SaveError"], MessageType.Error);
        }
    }

    private IEnumerable<ListCategoryDto> GetAllCategories()
    {
        var result = new List<ListCategoryDto>();
        AddCategoriesRecursive(_treeData, result);
        return result;
    }

    private static void AddCategoriesRecursive(IEnumerable<ListCategoryDto> categories, List<ListCategoryDto> result)
    {
        foreach (var category in categories)
        {
            if (category.Id > 0)
            {
                result.Add(category);
            }
            if (category.Children.Any())
            {
                AddCategoriesRecursive(category.Children, result);
            }
        }
    }
}
