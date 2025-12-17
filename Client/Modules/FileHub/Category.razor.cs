// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Category : ModuleBase
{
    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    [Inject]
    private Radzen.NotificationService NotificationService { get; set; } = default!;

    [Inject]
    private Radzen.ContextMenuService ContextMenuService { get; set; } = default!;

    [Inject]
    private Radzen.DialogService DialogService { get; set; } = default!;

    private const string SuccessNotificationMessage = "Success";
    private const string ErrorNotificationMessage = "Error";
    private List<ListCategoryDto> _treeData = [];
    private ListCategoryDto _rootNode = new() { Name = "<root categories>" };

    protected PagedResult<ListCategoryDto> Categories { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }
    protected ListCategoryDto? SelectedCategory { get; set; }
    protected CreateAndUpdateCategoryDto EditModel { get; set; } = new();
    protected bool ShowEditDialog { get; set; }
    protected bool IsAddingNew { get; set; }
    protected string DialogTitle { get; set; } = string.Empty;

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
                new() {
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
            new() {
                Text = "Add Child Category",
                Value = "add",
                Icon = "add",
            },
            new() {
                Text = "Edit Name",
                Value = "edit",
                Icon = "edit",
            },
            new() {
                Text = "Move Up",
                Value = "moveup",
                Icon = "arrow_upward",
                Disabled = !CanMoveUp(category),
            },
            new() {
                Text = "Move Down",
                Value = "movedown",
                Icon = "arrow_downward",
                Disabled = !CanMoveDown(category),
            },
        };

        // Only add delete option if category has no children
        if (!category.Children.Any())
        {
            menuItems.Add(new()
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
        // For root-level categories (ParentId is null), siblings are in TreeData
        if (category.ParentId is null)
        {
            return _treeData;
        }

        var parent = FindCategoryById([_rootNode], category.ParentId.Value);
        if (parent?.Children is List<ListCategoryDto> childrenList)
        {
            return childrenList;
        }

        return parent?.Children.ToList() ?? [];
    }

    private ListCategoryDto? FindCategoryById(List<ListCategoryDto> categories, int id)
    {
        foreach (var cat in categories)
        {
            if (cat.Id == id)
            {
                return cat;
            }

            var found = FindCategoryById(cat.Children.ToList(), id);

            if (found != null)
            {
                return found;
            }
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
            ParentId = SelectedCategory.Id == 0 ? null : SelectedCategory.Id, // If root node, ParentId is null
            ViewOrder = SelectedCategory.Children.Count,
            Children = []
        };

        // Add to parent's children
        SelectedCategory.Children.Add(newNode);

        // Automatically expand the parent node so the new child is visible
        SelectedCategory.IsExpanded = true;

        // Set editing state
        EditingNode = newNode;
        EditingNodeName = string.Empty;
        IsInlineAdding = true;
        IsInlineEditing = true;

        StateHasChanged();
    }

    private void OnNodeDoubleClick(ListCategoryDto? category)
    {
        if (category == null || category.Id == 0) return; // Don't allow editing root node

        // Cancel any existing inline editing
        CancelInlineEdit();

        // Set editing state
        EditingNode = category;
        EditingNodeName = category.Name;
        IsInlineAdding = false;
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

                // Update the temporary node in-place with the real ID and name
                EditingNode.Id = id;
                EditingNode.Name = EditingNodeName;

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = SuccessNotificationMessage,
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

                // Update the node name in-place
                EditingNode.Name = EditingNodeName;

                NotificationService.Notify(new Radzen.NotificationMessage
                {
                    Severity = Radzen.NotificationSeverity.Success,
                    Summary = SuccessNotificationMessage,
                    Detail = "Category updated successfully",
                    Duration = 3000,
                });
            }

            // Clear editing state without refreshing the tree
            EditingNode = null;
            EditingNodeName = string.Empty;
            IsInlineEditing = false;
            IsInlineAdding = false;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Saving Category {Error}", ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = ErrorNotificationMessage,
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

            if (EditingNode.ParentId is null)
            {
                parent = _rootNode;
            }
            else
            {
                parent = FindCategoryById([_rootNode], EditingNode.ParentId.Value);
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
        if (string.Equals(e.Key, "Enter", StringComparison.Ordinal))
        {
            await SaveInlineEdit();
        }
        else if (string.Equals(e.Key, "Escape", StringComparison.Ordinal))
        {
            CancelInlineEdit();
        }
    }

    private async Task MoveUp()
    {
        if (SelectedCategory == null)
        {
            return;
        }

        try
        {
            // Find siblings in the current tree structure
            var siblings = GetSiblings(SelectedCategory);
            var currentIndex = siblings.IndexOf(SelectedCategory);

            if (currentIndex <= 0) return;

            var current = SelectedCategory;
            var previous = siblings[currentIndex - 1];

            // Update on server using dedicated move endpoint
            await CategoryService.MoveUpAsync(current.Id, ModuleState.ModuleId);

            // Swap ViewOrder values locally
            (current.ViewOrder, previous.ViewOrder) = (previous.ViewOrder, current.ViewOrder);

            // Swap positions in the list
            siblings[currentIndex] = previous;
            siblings[currentIndex - 1] = current;

            await logger.LogInformation("Category Moved Up {Id}", SelectedCategory.Id);

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = SuccessNotificationMessage,
                Detail = "Category moved up",
                Duration = 3000,
            });

            // Clear selection
            SelectedCategory = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Moving Category Up {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = ErrorNotificationMessage,
                Detail = "Failed to move category",
                Duration = 4000,
            });
        }
    }

    private async Task MoveDown()
    {
        if (SelectedCategory == null)
        {
            return;
        }

        try
        {
            // Find siblings in the current tree structure
            var siblings = GetSiblings(SelectedCategory);
            var currentIndex = siblings.IndexOf(SelectedCategory);

            if (currentIndex < 0 || currentIndex >= siblings.Count - 1) return;

            var current = SelectedCategory;
            var next = siblings[currentIndex + 1];

            // Update on server using dedicated move endpoint
            await CategoryService.MoveDownAsync(current.Id, ModuleState.ModuleId);

            // Swap ViewOrder values locally
            (current.ViewOrder, next.ViewOrder) = (next.ViewOrder, current.ViewOrder);

            // Swap positions in the list
            siblings[currentIndex] = next;
            siblings[currentIndex + 1] = current;

            await logger.LogInformation("Category Moved Down {Id}", SelectedCategory.Id);

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = SuccessNotificationMessage,
                Detail = "Category moved down",
                Duration = 3000,
            });

            // Clear selection
            SelectedCategory = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Moving Category Down {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = ErrorNotificationMessage,
                Detail = "Failed to move category",
                Duration = 4000,
            });
        }
    }

    private async Task PromptDeleteCategory()
    {
        if (SelectedCategory == null)
        {
            return;
        }

        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to delete the category \"{SelectedCategory.Name}\"?",
            "Delete Category",
            new Radzen.ConfirmOptions
            {
                OkButtonText = "Yes, Delete",
                CancelButtonText = "Cancel",
                AutoFocusFirstElement = true
            });

        if (confirmed == true)
        {
            await DeleteCategory();
        }
        else
        {
            SelectedCategory = null;
        }
    }

    private async Task DeleteCategory()
    {
        if (SelectedCategory == null)
        {
            return;
        }

        try
        {
            var categoryToDelete = SelectedCategory;

            await CategoryService.DeleteAsync(categoryToDelete.Id, ModuleState.ModuleId);
            await logger.LogInformation("Category Deleted {Id}", categoryToDelete.Id);

            if (categoryToDelete.ParentId is null)
            {
                _treeData.Remove(categoryToDelete);
                _rootNode.Children.Remove(categoryToDelete);
            }
            else
            {
                var parent = FindCategoryById([_rootNode], categoryToDelete.ParentId.Value);
                if (parent != null)
                {
                    parent.Children.Remove(categoryToDelete);
                }
            }

            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Success,
                Summary = SuccessNotificationMessage,
                Detail = "Category deleted successfully",
                Duration = 4000,
            });

            SelectedCategory = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting Category {Id} {Error}", SelectedCategory.Id, ex.Message);
            NotificationService.Notify(new Radzen.NotificationMessage
            {
                Severity = Radzen.NotificationSeverity.Error,
                Summary = ErrorNotificationMessage,
                Detail = "Failed to delete category",
                Duration = 4000,
            });
        }
    }

    private void CreateTreeStructure()
    {
        if (Categories.Items is null || !Categories.Items.Any())
        {
            _treeData = [];

            // Create root node with empty children
            _rootNode = new ListCategoryDto
            {
                Id = 0,
                Name = "<root categories>",
                ParentId = -1,
                ViewOrder = 0,
                IsExpanded = true,
                Children = []
            };
            return;
        }

        foreach (var category in Categories.Items)
        {
            category.Children.Clear();
        }

        var categoryDict = Categories.Items.ToDictionary(c => c.Id, c => c);

        _treeData = Categories.Items
            .Where(c => c.ParentId is null || !categoryDict.ContainsKey(c.ParentId.Value))
            .OrderBy(c => c.ViewOrder)
            .ThenBy(c => c.Name, StringComparer.Ordinal)
            .ToList();

        foreach (var category in Categories.Items)
        {
            if (category.ParentId is not null && categoryDict.TryGetValue(category.ParentId.Value, out var parent))
            {
                parent.Children.Add(category);
            }
        }

        SortChildren(_treeData);

        // Create root node with TreeData as children
        _rootNode = new ListCategoryDto
        {
            Id = 0,
            Name = "<root categories>",
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

    private static Task OnNodeExpand(Radzen.TreeExpandEventArgs args)
    {
        if (args.Value is ListCategoryDto category)
        {
            category.IsExpanded = true;
        }
        return Task.CompletedTask;
    }

    private static void OnNodeCollapse(Radzen.TreeEventArgs args)
    {
        if (args.Value is ListCategoryDto category)
        {
            category.IsExpanded = false;
        }
    }
}
