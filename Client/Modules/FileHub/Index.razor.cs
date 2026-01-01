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

    private void OnDownloadClick(ListFileDto file)
    {
        // Increment the counter in the UI immediately for instant feedback
        file.Downloads++;
        StateHasChanged();
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

        // Use the serve endpoint for image display (no download counter increment)
        return $"/api/ictace/fileHub/files/serve/{Uri.EscapeDataString(imageName)}";
    }

    private string GetDownloadUrl(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        // Use the serve endpoint with download=true to increment counter
        return $"/api/ictace/fileHub/files/serve/{Uri.EscapeDataString(fileName)}?download=true";
    }
}
