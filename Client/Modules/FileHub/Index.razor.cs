// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public partial class Index
{
    [Inject] protected Services.IFileService FileService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

    public override List<Resource> Resources =>
    [
        new Stylesheet(ModulePath() + "Module.css"),
        new Script(ModulePath() + "Module.js"),
        new Script("_content/Radzen.Blazor/Radzen.Blazor.js")
    ];

    private List<ListFileDto>? _files;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _isLoading = true;
            var pagedResult = await FileService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue).ConfigureAwait(true);
            _files = pagedResult?.Items?.ToList();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading Files {Error}", ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task Delete(ListFileDto file)
    {
        try
        {
            await FileService.DeleteAsync(file.Id, ModuleState.ModuleId).ConfigureAwait(true);
            await logger.LogInformation("File Deleted {Id}", file.Id).ConfigureAwait(true);

            var pagedResult = await FileService.ListAsync(ModuleState.ModuleId, pageNumber: 1, pageSize: int.MaxValue).ConfigureAwait(true);
            _files = pagedResult?.Items?.ToList();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Deleting File {Id} {Error}", file.Id, ex.Message).ConfigureAwait(true);
            AddModuleMessage(Localizer["Message.DeleteError"], MessageType.Error);
        }
    }

    private void NavigateToEdit(int fileId)
    {
        NavigationManager.NavigateTo(EditUrl("id", fileId.ToString()));
    }

    private void NavigateToAdd()
    {
        NavigationManager.NavigateTo(EditUrl("Add"));
    }

    private string GetImageUrl(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            return string.Empty;
        }

        // Use the serve endpoint - moduleId will be looked up server-side
        return $"/api/ictace/fileHub/files/serve/{Uri.EscapeDataString(imageName)}";
    }

    private string GetDownloadUrl(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        // Use the serve endpoint - moduleId will be looked up server-side
        return $"/api/ictace/fileHub/files/serve/{Uri.EscapeDataString(fileName)}";
    }
}
