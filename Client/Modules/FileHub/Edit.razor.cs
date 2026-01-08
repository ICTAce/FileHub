// Licensed to ICTAce under the MIT license.

using Microsoft.AspNetCore.Components.Forms;
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
    private bool _isSaving;
    
    private IBrowserFile? _selectedFile;
    private IBrowserFile? _selectedImage;

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
                    
                    if (file.CategoryIds.Any())
                    {
                        var selectedCats = GetAllCategories().Where(c => file.CategoryIds.Contains(c.Id)).ToList();
                        _selectedCategories = selectedCats.Cast<object>();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading File {Id} {Error}", _id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
    }

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        try
        {
            var file = e.File;
            
            const long maxFileSize = 100 * 1024 * 1024;
            if (file.Size > maxFileSize)
            {
                AddModuleMessage("File size exceeds 100MB limit", MessageType.Error);
                _selectedFile = null;
                return;
            }

            _selectedFile = file;
            
            if (string.IsNullOrEmpty(_name))
            {
                _name = Path.GetFileNameWithoutExtension(file.Name);
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            AddModuleMessage("Error selecting file", MessageType.Error);
            _selectedFile = null;
        }
    }

    private void OnImageSelected(InputFileChangeEventArgs e)
    {
        try
        {
            var file = e.File;
            
            const long maxImageSize = 10 * 1024 * 1024;
            if (file.Size > maxImageSize)
            {
                AddModuleMessage("Image size exceeds 10MB limit", MessageType.Error);
                _selectedImage = null;
                return;
            }

            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                AddModuleMessage("Please select a valid image file", MessageType.Error);
                _selectedImage = null;
                return;
            }

            _selectedImage = file;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            AddModuleMessage("Error selecting image", MessageType.Error);
            _selectedImage = null;
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    private static string GetDisplayFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        var parts = fileName.Split('_', 2);
        
        if (parts.Length >= 2 && parts[0].Length == 36 && Guid.TryParse(parts[0], out _))
        {
            return parts[1];
        }

        return fileName;
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

    private async Task Save()
    {
        try
        {
            _validated = true;
            var interop = new Oqtane.UI.Interop(JSRuntime);
            
            if (PageState.Action == "Add" && _selectedFile == null)
            {
                AddModuleMessage("Please select a file to upload", MessageType.Warning);
                return;
            }
            
            if (await interop.FormValid(form))
            {
                _isSaving = true;
                StateHasChanged();

                string uploadedFileName = _fileName;
                string uploadedImageName = _imageName;
                string fileSize = _fileSize;

                try
                {
                    if (_selectedFile != null)
                    {
                        const long maxFileSize = 100 * 1024 * 1024;
                        using var stream = _selectedFile.OpenReadStream(maxFileSize);
                        uploadedFileName = await FileService.UploadFileAsync(ModuleState.ModuleId, stream, _selectedFile.Name).ConfigureAwait(true);
                        fileSize = FormatFileSize(_selectedFile.Size);
                    }

                    if (_selectedImage != null)
                    {
                        const long maxImageSize = 10 * 1024 * 1024;
                        using var stream = _selectedImage.OpenReadStream(maxImageSize);
                        uploadedImageName = await FileService.UploadFileAsync(ModuleState.ModuleId, stream, _selectedImage.Name).ConfigureAwait(true);
                    }

                    var selectedCategoryIds = _selectedCategories?
                        .OfType<ListCategoryDto>()
                        .Where(c => c.Id > 0)
                        .Select(c => c.Id)
                        .ToList() ?? [];

                    if (string.Equals(PageState.Action, "Add", StringComparison.Ordinal))
                    {
                        var dto = new CreateAndUpdateFileDto
                        {
                            Name = _name,
                            FileName = uploadedFileName,
                            ImageName = uploadedImageName,
                            Description = _description,
                            FileSize = fileSize,
                            Downloads = 0,
                            CategoryIds = selectedCategoryIds
                        };
                        var id = await FileService.CreateAsync(ModuleState.ModuleId, dto).ConfigureAwait(true);
                        await logger.LogInformation("File Created {Id}", id).ConfigureAwait(true);
                        AddModuleMessage("File uploaded successfully", MessageType.Success);
                    }
                    else
                    {
                        var dto = new CreateAndUpdateFileDto
                        {
                            Name = _name,
                            FileName = uploadedFileName,
                            ImageName = uploadedImageName,
                            Description = _description,
                            FileSize = fileSize,
                            Downloads = 0,
                            CategoryIds = selectedCategoryIds
                        };
                        var id = await FileService.UpdateAsync(_id, ModuleState.ModuleId, dto).ConfigureAwait(true);
                        await logger.LogInformation("File Updated {Id}", id).ConfigureAwait(true);
                        AddModuleMessage("File updated successfully", MessageType.Success);
                    }
                    
                    NavigationManager.NavigateTo(NavigateUrl());
                }
                catch (Exception uploadEx)
                {
                    await logger.LogError(uploadEx, "Error Uploading File {Error}", uploadEx.Message).ConfigureAwait(true);
                    AddModuleMessage("Error uploading file. Please try again.", MessageType.Error);
                }
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
        finally
        {
            _isSaving = false;
            StateHasChanged();
        }
    }

    private async Task Delete()
    {
        try
        {
            await FileService.DeleteAsync(_id, ModuleState.ModuleId).ConfigureAwait(true);
            await logger.LogInformation("File Deleted {Id}", _id).ConfigureAwait(true);
            NavigationManager.NavigateTo(NavigateUrl());
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting File {Id} {Error}", _id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
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
