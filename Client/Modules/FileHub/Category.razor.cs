// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Category : ModuleBase
{
    private List<ListCategoryDto> TreeData = new();
    private ListCategoryDto RootNode = new() { Name = "All Categories" };

    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    [Inject]
    private Radzen.NotificationService NotificationService { get; set; } = default!;

    [Inject]
    private Radzen.ContextMenuService ContextMenuService { get; set; } = default!;

    protected PagedResult<ListCategoryDto> Categories { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }
    protected ListCategoryDto? SelectedCategory { get; set; }
    protected CreateAndUpdateCategoryDto EditModel { get; set; } = new();
    protected bool ShowDeleteConfirmation { get; set; }
    protected bool ShowEditDialog { get; set; }
    protected bool IsAddingNew { get; set; }
    protected string DialogTitle { get; set; } = string.Empty;
    
    // Inline editing properties
    protected ListCategoryDto? EditingNode { get; set; }
    protected string EditingNodeName { get; set; } = string.Empty;
    protected bool IsInlineEditing { get; set; }
    protected bool IsInlineAdding { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
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

    private void ShowContextMenu(MouseEventArgs args, ListCategoryDto? category)
    {
        if (category == null) return;

        SelectedCategory = category;

        // For root node, only show "Add Child Category"
        if (category.Id == 0)
        {
            var rootMenuItems = new List<Radzen.ContextMenuItem>
            {
                new Radzen.ContextMenuItem
                {
                    Text = "Add Category",
                    Value = "add",
                    Icon = "add",
                }
            };

            ContextMenuService.Open(args, rootMenuItems, OnContextMenuClick);
            return;
        }

        var menuItems = new List<Radzen.ContextMenuItem>
        {
            new Radzen.ContextMenuItem
            {
                Text = "Add Child Category",
                Value = "add",
                Icon = "add",
            },
            new Radzen.ContextMenuItem
            {
                Text = "Edit Name",
                Value = "edit",
                Icon = "edit",
            },
            new Radzen.ContextMenuItem
            {
                Text = "Move Up",
                Value = "moveup",
                Icon = "arrow_upward",
                Disabled = !CanMoveUp(category),
            },
            new Radzen.ContextMenuItem
            {
                Text = "Move Down",
                Value = "movedown",
                Icon = "arrow_downward",
                Disabled = !CanMoveDown(category),
            },
        };

        // Only add delete option if category has no children
        if (!category.Children.Any())
        {
            menuItems.Add(new Radzen.ContextMenuItem
            {
                Text = "Delete",
                Value = "delete",
                Icon = "delete",
                Disabled = false,
            });
        }

        ContextMenuService.Open(args, menuItems, OnContextMenuClick);
    }

    private void OnContextMenuClick(Radzen.MenuItemEventArgs args)
    {
        var action = args.Value?.ToString();

        switch (action)
        {
            case "add":
                AddChildCategoryInline();
                break;
            case "edit":
                EditCategoryInline();
                break;
            case "moveup":
                _ = MoveUp();
                break;
            case "movedown":
                _ = MoveDown();
                break;
            case "delete":
                PromptDeleteCategory();
                break;
        }

        ContextMenuService.Close();
    }

    private bool CanMoveUp(ListCategoryDto category)
    {
        var siblings = GetSiblings(category);
        var index = siblings.IndexOf(category);
        return index > 0;
    }

    private bool CanMoveDown(ListCategoryDto category)
    {
        var siblings = GetSiblings(category);
        var index = siblings.IndexOf(category);
        return index >= 0 && index < siblings.Count - 1;
    }

    private List<ListCategoryDto> GetSiblings(ListCategoryDto category)
    {
        // For root-level categories (ParentId == 0), siblings are in TreeData
        if (category.ParentId == 0)
        {
            return TreeData;
        }

        var parent = FindCategoryById(new List<ListCategoryDto> { RootNode }, category.ParentId);
        return parent?.Children ?? new List<ListCategoryDto>();
    }

    private ListCategoryDto? FindCategoryById(List<ListCategoryDto> categories, int id)
    {
        foreach (var cat in categories)
        {
            if (cat.Id == id) return cat;
            var found = FindCategoryById(cat.Children, id);
            if (found != null) return found;
        }
        return null;
    }

    private void AddChildCategoryInline()
    {
        if (SelectedCategory == null) return;

        // Cancel any existing inline editing
        CancelInlineEdit();

        // Create a temporary new node
        var newNode = new ListCategoryDto
        {
            Id = -1, // Temporary ID
            Name = string.Empty,
            ParentId = SelectedCategory.Id == 0 ? 0 : SelectedCategory.Id, // If root node, ParentId is 0
            ViewOrder = SelectedCategory.Children.Count,
            Children = new List<ListCategoryDto>()
        };

        // Add to parent's children
        SelectedCategory.Children.Add(newNode);

        // Set editing state
        EditingNode = newNode;
        EditingNodeName = string.Empty;
        IsInlineAdding = true;
        IsInlineEditing = true;

        StateHasChanged();
    }

    private void EditCategoryInline()
    {
        if (SelectedCategory == null || SelectedCategory.Id == 0) return; // Don't allow editing root node

        // Cancel any existing inline editing
        CancelInlineEdit();

        // Set editing state
        EditingNode = SelectedCategory;
        EditingNodeName = SelectedCategory.Name;
        IsInlineAdding = false;
        IsInlineEditing = true;

        StateHasChanged();
    }

    private async Task SaveInlineEdit()
    {
        if (EditingNode == null || string.IsNullOrWhiteSpace(EditingNodeName))
        {
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Warning,
                Summary = "Validation Error",
                Detail = "Category name is required",
                Duration = 4000,
            });
            return;
        }

        try
        {
            if (IsInlineAdding)
            {
                // Create new category
                var createDto = new CreateAndUpdateCategoryDto
                {
                    Name = EditingNodeName,
                    ViewOrder = EditingNode.ViewOrder,
                    ParentId = EditingNode.ParentId,
                };

                var id = await CategoryService.CreateAsync(ModuleState.ModuleId, createDto);
                await logger.LogInformation("Category Created {Id}", id);

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Category created successfully",
                    Duration = 3000,
                });
            }
            else
            {
                // Update existing category
                var updateDto = new CreateAndUpdateCategoryDto
                {
                    Name = EditingNodeName,
                    ViewOrder = EditingNode.ViewOrder,
                    ParentId = EditingNode.ParentId,
                };

                await CategoryService.UpdateAsync(EditingNode.Id, ModuleState.ModuleId, updateDto);
                await logger.LogInformation("Category Updated {Id}", EditingNode.Id);

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Category updated successfully",
                    Duration = 3000,
                });
            }

            // Clear editing state
            CancelInlineEdit();

            // Refresh tree
            await RefreshCategories();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Saving Category {Error}", ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Failed to save category",
                Duration = 4000,
            });
        }
    }

    private void CancelInlineEdit()
    {
        if (IsInlineAdding && EditingNode != null)
        {
            // Remove the temporary node from the tree
            ListCategoryDto? parent;
            
            if (EditingNode.ParentId == 0)
            {
                parent = RootNode;
            }
            else
            {
                parent = FindCategoryById(new List<ListCategoryDto> { RootNode }, EditingNode.ParentId);
            }
            
            if (parent != null)
            {
                parent.Children.Remove(EditingNode);
            }
        }

        EditingNode = null;
        EditingNodeName = string.Empty;
        IsInlineEditing = false;
        IsInlineAdding = false;

        StateHasChanged();
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SaveInlineEdit();
        }
        else if (e.Key == "Escape")
        {
            CancelInlineEdit();
        }
    }

    private void AddChildCategory()
    {
        IsAddingNew = true;
        ShowEditDialog = true;
        ShowDeleteConfirmation = false;
        DialogTitle = $"Add Child Category to '{SelectedCategory?.Name}'";

        EditModel = new CreateAndUpdateCategoryDto
        {
            Name = string.Empty,
            ViewOrder = 0,
            ParentId = SelectedCategory?.Id ?? 0,
        };

        StateHasChanged();
    }

    private void EditCategory()
    {
        if (SelectedCategory == null) return;

        IsAddingNew = false;
        ShowEditDialog = true;
        ShowDeleteConfirmation = false;
        DialogTitle = $"Edit Category '{SelectedCategory.Name}'";

        EditModel = new CreateAndUpdateCategoryDto
        {
            Name = SelectedCategory.Name,
            ViewOrder = SelectedCategory.ViewOrder,
            ParentId = SelectedCategory.ParentId,
        };

        StateHasChanged();
    }

    private async Task MoveUp()
    {
        if (SelectedCategory == null) return;

        try
        {
            // Get fresh data to find current siblings
            var freshCategories = await CategoryService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue);

            // Find siblings with same parent
            var siblings = freshCategories.Items?
                .Where(c => c.ParentId == SelectedCategory.ParentId)
                .OrderBy(c => c.ViewOrder)
                .ThenBy(c => c.Name)
                .ToList() ?? new List<ListCategoryDto>();

            var currentIndex = siblings.FindIndex(c => c.Id == SelectedCategory.Id);

            if (currentIndex <= 0) return;

            var current = siblings[currentIndex];
            var previous = siblings[currentIndex - 1];

            // Swap ViewOrder values
            var tempViewOrder = current.ViewOrder;

            // Update current category
            await CategoryService.UpdateAsync(current.Id, ModuleState.ModuleId,
                new CreateAndUpdateCategoryDto
                {
                    Name = current.Name,
                    ViewOrder = previous.ViewOrder,
                    ParentId = current.ParentId,
                });

            // Update previous sibling
            await CategoryService.UpdateAsync(previous.Id, ModuleState.ModuleId,
                new CreateAndUpdateCategoryDto
                {
                    Name = previous.Name,
                    ViewOrder = tempViewOrder,
                    ParentId = previous.ParentId,
                });

            await logger.LogInformation("Category Moved Up {Id}", SelectedCategory.Id);

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Category moved up",
                Duration = 3000,
            });

            // Clear selection and refresh
            SelectedCategory = null;
            await RefreshCategories();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Moving Category Up {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Failed to move category",
                Duration = 4000,
            });
        }
    }

    private async Task MoveDown()
    {
        if (SelectedCategory == null) return;

        try
        {
            // Get fresh data to find current siblings
            var freshCategories = await CategoryService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue);

            // Find siblings with same parent
            var siblings = freshCategories.Items?
                .Where(c => c.ParentId == SelectedCategory.ParentId)
                .OrderBy(c => c.ViewOrder)
                .ThenBy(c => c.Name)
                .ToList() ?? new List<ListCategoryDto>();

            var currentIndex = siblings.FindIndex(c => c.Id == SelectedCategory.Id);

            if (currentIndex < 0 || currentIndex >= siblings.Count - 1) return;

            var current = siblings[currentIndex];
            var next = siblings[currentIndex + 1];

            // Swap ViewOrder values
            var tempViewOrder = current.ViewOrder;

            // Update current category
            await CategoryService.UpdateAsync(current.Id, ModuleState.ModuleId,
                new CreateAndUpdateCategoryDto
                {
                    Name = current.Name,
                    ViewOrder = next.ViewOrder,
                    ParentId = current.ParentId,
                });

            // Update next sibling
            await CategoryService.UpdateAsync(next.Id, ModuleState.ModuleId,
                new CreateAndUpdateCategoryDto
                {
                    Name = next.Name,
                    ViewOrder = tempViewOrder,
                    ParentId = next.ParentId,
                });

            await logger.LogInformation("Category Moved Down {Id}", SelectedCategory.Id);

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Category moved down",
                Duration = 3000,
            });

            // Clear selection and refresh
            SelectedCategory = null;
            await RefreshCategories();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Moving Category Down {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Failed to move category",
                Duration = 4000,
            });
        }
    }

    private async Task SaveCategory()
    {
        if (string.IsNullOrWhiteSpace(EditModel.Name))
        {
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Warning,
                Summary = "Validation Error",
                Detail = "Category name is required",
                Duration = 4000,
            });
            return;
        }

        try
        {
            if (IsAddingNew)
            {
                // Create new category
                var id = await CategoryService.CreateAsync(ModuleState.ModuleId, EditModel);
                await logger.LogInformation("Category Created {Id}", id);

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Category created successfully",
                    Duration = 4000,
                });
            }
            else if (SelectedCategory != null)
            {
                // Update existing category
                await CategoryService.UpdateAsync(SelectedCategory.Id, ModuleState.ModuleId, EditModel);
                await logger.LogInformation("Category Updated {Id}", SelectedCategory.Id);

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Category updated successfully",
                    Duration = 4000,
                });
            }

            ShowEditDialog = false;
            IsAddingNew = false;
            SelectedCategory = null;
            await RefreshCategories();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Saving Category {Error}", ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Failed to save category",
                Duration = 4000,
            });
        }
    }

    private void CancelEdit()
    {
        ShowEditDialog = false;
        IsAddingNew = false;
        SelectedCategory = null;
    }

    private void PromptDeleteCategory()
    {
        ShowDeleteConfirmation = true;
        ShowEditDialog = false;
        StateHasChanged();
    }

    private async Task ConfirmDeleteCategory()
    {
        if (SelectedCategory == null) return;

        try
        {
            await CategoryService.DeleteAsync(SelectedCategory.Id, ModuleState.ModuleId);
            await logger.LogInformation("Category Deleted {Id}", SelectedCategory.Id);

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Category deleted successfully",
                Duration = 4000,
            });

            SelectedCategory = null;
            ShowDeleteConfirmation = false;
            await RefreshCategories();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting Category {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Failed to delete category",
                Duration = 4000,
            });
        }
    }

    private void CancelDelete()
    {
        ShowDeleteConfirmation = false;
        SelectedCategory = null;
    }

    private async Task RefreshCategories()
    {
        IsLoading = true;
        try
        {
            Categories = await CategoryService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue);
            CreateTreeStructure();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to reload categories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void CreateTreeStructure()
    {
        if (Categories.Items is null || !Categories.Items.Any())
        {
            TreeData = new();
            
            // Create root node with empty children
            RootNode = new ListCategoryDto
            {
                Id = 0,
                Name = "All Categories",
                ParentId = -1,
                ViewOrder = 0,
                Children = new List<ListCategoryDto>()
            };
            return;
        }

        foreach (var category in Categories.Items)
        {
            category.Children.Clear();
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

        // Create root node with TreeData as children
        RootNode = new ListCategoryDto
        {
            Id = 0,
            Name = "All Categories",
            ParentId = -1,
            ViewOrder = 0,
            Children = TreeData
        };
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
